using Android.App;
using Android.OS;

namespace Microsoft.Maui
{
	/// <summary>
	/// Allow to get Android Activity lifecycle callbacks.
	/// </summary>
	public interface IAndroidLifecycleHandler : IPlatformLifecycleHandler
	{
		/// <summary>
		/// Called when the activity is starting.
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		/// <param name="savedInstanceState">Previous state saved</param>
		void OnCreate(Activity activity, Bundle? savedInstanceState);

		/// <summary>
		/// Called when the activity had been stopped, but is now again being displayed to the user. 
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		void OnStart(Activity activity);

		/// <summary>
		/// Called for your activity to start interacting with the user.
		/// This is an indicator that the activity became active and ready to receive input.
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		void OnResume(Activity activity);

		/// <summary>
		/// Called as part of the activity lifecycle when the user no longer actively interacts with the activity,
		/// but it is still visible on screen.
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		void OnPause(Activity activity);

		/// <summary>
		/// Called when you are no longer visible to the user.
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		void OnStop(Activity activity);

		/// <summary>
		/// This can happen either because the activity is finishing, or because the system is
		/// temporarily destroying this instance of the activity to save space. 
		/// </summary>
		/// <param name="activity">The activity on which we receive lifecycle events callbacks</param>
		void OnDestroy(Activity activity);
	}
}