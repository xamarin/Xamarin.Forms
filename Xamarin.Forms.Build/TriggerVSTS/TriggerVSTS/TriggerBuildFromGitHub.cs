using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using Xamarin.Provisioning;

namespace TriggerVSTSBuild
{
	public static class TriggerBuildFromGitHub
	{
		static HttpResponseMessage Error (this HttpRequestMessage req, string error)
		{
			s_log?.Error (error);
			return req.CreateResponse (HttpStatusCode.OK, new {
				error
			});
		}

		static HttpClient gitHubClient;
		static TraceWriter s_log;

		[FunctionName ("TriggerBuildFromGitHub")]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (WebHookType = "github")]HttpRequestMessage req,
			TraceWriter log
		)
		{
			var buildCommands = Environment.GetEnvironmentVariable ("BuildCommands");
			var commands = new CommandParser ().ParseAllCommands (buildCommands);
			
			string eventType = req.Headers.GetValues ("X-GitHub-Event").First ();

			switch (eventType) {
			case "issue_comment":
				return await HandleIssueCommentAsync (req, log, commands);
			case "pull_request":
				return await HandlePullRequestAsync (req, log, commands);
			default:
				return req.Error ($"Don't know how to handle {eventType} events.");
			}
		}

		static async Task<HttpResponseMessage> HandlePullRequestAsync (HttpRequestMessage req, TraceWriter log, IList<Command> commands)
		{
			dynamic data = await req.Content.ReadAsAsync<object> ();
			var action = (string)data.action;
			var command = commands.FirstOrDefault (c => c.Comment.Equals ("build"));
			switch (action) {
			case "created":
			case "synchronize":
				var pullRequestNumber = (int)data.number;
				var sender = (string)data.pull_request.user.login;
				var repoName = (string)data.repository.full_name;

				log.Info ($"Evaluating building pull request {pullRequestNumber} on {repoName} from user {sender}.");

				return await CheckPermissionsAndQueueBuildsAsync (req, pullRequestNumber, sender, repoName, command);
			default:
				return req.Error ($"We don't care about pull request events other than created or synchronize, received {action}.");
			}
		}

		static async Task<HttpResponseMessage> HandleIssueCommentAsync (HttpRequestMessage req, TraceWriter log, IList<Command> commands)
		{
			dynamic data = await req.Content.ReadAsAsync<object> ();

			var sender = (string)data.comment.user.login;

			// This isn't a PR.
			if (data.issue.pull_request == null)
				return req.Error ($"Comment by {sender} is not a pull request comment, ignoring.");

			// Extract github comment from request body
			string gitHubComment = data.comment.body;
			//Check if we have this configuration
			var command = commands.FirstOrDefault (c => c.Comment.Equals (gitHubComment.Trim ()));
			if (command == null)
				return req.Error ($"{sender} did not request a build, ignoring.");
			
			var pullRequestNumber = (int)data.issue.number;
			var repoName = (string)data.repository.full_name;

			log.Info ($"Evaluating whether to build pull request {pullRequestNumber} on repo {repoName} at request of {sender}.");

			return await CheckPermissionsAndQueueBuildsAsync (req, pullRequestNumber, sender, repoName, command);
		}

		static async Task<HttpResponseMessage> CheckPermissionsAndQueueBuildsAsync (
			HttpRequestMessage req,
			int pullRequestNumber,
			string sender,
			string repoName,
			Command command
		)
		{
			var permissionError = await CheckIfUserHasPermissionsAsync (req, repoName, sender);
			if (permissionError != null)
				return permissionError;

			var error = GetAndValidateVstsParameters (
				req,
				out var vstsProjectName,
				out var vstsPat,
				out var vstsBuildDefinitionString,
				out var vstsCollectionString,
				out var vstsBuildDefinitionIds
			);

			if (command.Comment == "build" && command.AssociatedBuilds == null)
				command.AssociatedBuilds = new List<int> (vstsBuildDefinitionIds);

			if (error != null)
				return error;

			var sourceBranch = $"refs/pull/{pullRequestNumber}/merge";
			var builds = await QueueBuildsAsync (
				vstsProjectName,
				vstsPat,
				vstsCollectionString,
				command.AssociatedBuilds,
				sourceBranch
			);

			return req.CreateResponse (HttpStatusCode.OK, new {
				queuedBuildIds = builds.Select (b => b.Id),
				queuedBuildUris = builds.Select (b => b.Uri.ToString ()),
				queuedBy = sender,
				hookVersion = ThisAssembly.Git.Commit
			});
		}

		static HttpResponseMessage GetAndValidateVstsParameters (
			HttpRequestMessage req,
			out string vstsProjectName,
			out string vstsPat,
			out string vstsBuildDefinitionString,
			out string vstsCollectionString,
			out List<int> vstsBuildDefinitionIds)
		{
			vstsProjectName = Environment.GetEnvironmentVariable ("VSTS_PROJECT_NAME");
			vstsPat = Environment.GetEnvironmentVariable ("VSTS_ACCESS_TOKEN");
			vstsBuildDefinitionString = Environment.GetEnvironmentVariable ("VSTS_BUILD_DEFINITION_IDS");
			vstsCollectionString = Environment.GetEnvironmentVariable ("VSTS_COLLECTION_URI");
			vstsBuildDefinitionIds = null;

			if (string.IsNullOrWhiteSpace (vstsProjectName))
				return req.Error ("VSTS project name (VSTS_PROJECT_NAME) must be specified.");

			if (string.IsNullOrWhiteSpace (vstsPat))
				return req.Error ("VSTS PAT (VSTS_ACCESS_TOKEN) must be specified.");

			if (string.IsNullOrWhiteSpace (vstsBuildDefinitionString))
				return req.Error ("VSTS build definition list (VSTS_BUILD_DEFINITION_IDS) must be specified.");

			vstsBuildDefinitionIds = ParseVstsBuildDefinitions (vstsBuildDefinitionString);

			if (!vstsBuildDefinitionIds.Any ())
				return req.Error ("At least one valid VSTS build definition ID (VSTS_BUILD_DEFINITION_IDS) must be specified.");

			if (!Uri.TryCreate (vstsCollectionString, UriKind.Absolute, out var vstsCollectionUri))
				return req.Error ("VSTS collection (VSTS_COLLECTION_URI) must be a valid absolute URI.");

			return null;
		}

		static async Task<List<(int Id, string Uri)>> QueueBuildsAsync (
			string vstsProjectName,
			string vstsPat,
			string vstsCollectionString,
			IList<int> vstsBuildDefinitionIds,
			string sourceBranch)
		{
			var builds = new List<(int Id, string Uri)> ();

			foreach (var vstsBuildDefinitionId in vstsBuildDefinitionIds) {
				var queuedBuild = await VstsBuild.QueueBuildAsync (
					vstsCollectionString,
					vstsPat,
					vstsBuildDefinitionId,
					vstsProjectName,
					sourceBranch: sourceBranch
				);

				builds.Add ((
					queuedBuild ["Content"].Value<int> ("id"),
					queuedBuild ["Content"].Value<JObject> ("_links").Value<JObject> ("web").Value<string> ("href")
				));
			}

			return builds;
		}

		static List<int> ParseVstsBuildDefinitions (string vstsBuildDefinitionString)
		{
			var vstsBuildDefinitionStrings = vstsBuildDefinitionString.Split (",".ToCharArray (), StringSplitOptions.RemoveEmptyEntries)
										  .Select (s => s.Trim ());
			var vstsBuildDefinitionIds = new List<int> ();
			foreach (var buildDefinitionString in vstsBuildDefinitionStrings) {
				if (int.TryParse (buildDefinitionString, out var buildDefinitionId)) {
					vstsBuildDefinitionIds.Add (buildDefinitionId);
				}
			}

			return vstsBuildDefinitionIds;
		}

		static async Task<HttpResponseMessage> CheckIfUserHasPermissionsAsync (HttpRequestMessage req, string repoName, string user)
		{
			var githubClient = GetGitHubClient ();

			if (githubClient == null)
				return req.Error ("Could not create GitHub client, make sure you specify GITHUB_API_TOKEN.");

			var permissionRequestUrl = $"repos/{repoName}/collaborators/{user}/permission";
			var result = await gitHubClient.GetAsync (permissionRequestUrl);

			if (!result.IsSuccessStatusCode)
				return req.Error ($"GitHub API returned: {result.StatusCode} {result.ReasonPhrase}.");

			var json = JObject.Parse (await result.Content.ReadAsStringAsync ());
			var permission = json.Value<string> ("permission");
			var hasPermissions = permission == "admin" || permission == "write";

			if (hasPermissions)
				return null;
			return req.Error ($"User {user} does not have permission to build pull requests on {repoName}.");
		}

		static HttpClient GetGitHubClient ()
		{
			if (gitHubClient != null)
				return gitHubClient;

			var githubToken = Environment.GetEnvironmentVariable ("GITHUB_API_TOKEN");

			if (string.IsNullOrWhiteSpace (githubToken))
				return null;

			gitHubClient = new HttpClient {
				BaseAddress = new Uri ("https://api.github.com/"),
			};
			gitHubClient.DefaultRequestHeaders.Add ("User-Agent", "Trigger VSTS Build From GitHub");
			gitHubClient.DefaultRequestHeaders.Add ("Authorization", $"token {githubToken}");

			return gitHubClient;
		}
	}
}
