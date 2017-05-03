using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.WPF.Helpers
{
    public interface IApplicationData
    {
        Stream GetFileAsync(string propertyStoreFile);
        Stream CreateFileAsync(string propertyStoreFile);
    }

    public class ApplicationData : IApplicationData
    {
		private string LocalPath=>Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		static ApplicationData s_applicationData;

        public static IApplicationData Instance => s_applicationData ?? (s_applicationData = new ApplicationData());

        public Stream GetFileAsync(string propertyStoreFile)
        {
            return File.OpenRead(Path.Combine(LocalPath,propertyStoreFile));
        }

        public Stream CreateFileAsync(string propertyStoreFile)
        {
            return File.Open(Path.Combine(LocalPath,propertyStoreFile), System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite);
        }
    }
}
