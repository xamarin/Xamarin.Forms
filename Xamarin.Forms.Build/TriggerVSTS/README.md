# TriggerVSTSBuild

TriggerVSTSBuild is an Azure function that provides a convenient way to
trigger VSTS builds from GitHub, both by authorized users commenting on
a pull request and via automatic builds of pull requests made by
authorized users, even across forks.

Authorized users are checked via the GitHub API, so modifications only
need to be in one place.

## Deployment

1. Install the [Azure Functions tools][tools] for VS2017.
1. Right-click on the project, click Publish.
1. Select an existing configured Azure Functions service or create a new one.

This may also work from Visual Studio for Mac, but I haven't explored those tools.

## Configuration

The Azure Functions service must be configured with a few app settings before
the function will work:

|               Variable Name | Description                                                                       |
|----------------------------:|-----------------------------------------------------------------------------------|
|         `VSTS_PROJECT_NAME` | The VSTS project name. For example, `DevDiv`.                                     |
|         `VSTS_ACCESS_TOKEN` | A VSTS PAT. Needs permissions to execute builds and nothing else.                 |
| `VSTS_BUILD_DEFINITION_IDS` | A comma-separated list of VSTS build definition IDs to trigger.                   |
|       `VSTS_COLLECTION_URI` | The VSTS instance URI. Must be an absolute URI.                                   |
|          `GITHUB_API_TOKEN` | A GitHub API token/PAT. Used to validate users have permission to start builds.   |

## Configuring in GitHub

1. Go to the Azure portal for your deployed function and select the function.
1. Use the Get Function Url and Get GitHub Secret links to get the URL for the function and the GitHub secret.
   They'll look something like this:
     * URL: `https://triggervstsbuild.azurewebsites.net/api/TriggerBuildFromGitHub?clientId=default`
     * Secret: `V11gr4u190020Yov123kuKSGGGWfj8aW1B/zGVO5a4ApZReqX3AQQQQ==`
1. Add a new webhook on the repository with the URL and secret you just obtained. Turn on only the issue comment
   and pull request events--you can send all events, but anything other than those 2 will be ignored.

[tools]: https://marketplace.visualstudio.com/items?itemName=AndrewBHall-MSFT.AzureFunctionToolsforVisualStudio2017