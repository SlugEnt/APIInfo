using System;

namespace SlugEnt.APIInfo
{
	public class APIInfoBase {
		private const string ROOT_PATH = "info";

		private static string _rootPath = ROOT_PATH;

		public string InfoRootPath {
			get { return _rootPath; }
			set {
				if ( _rootPath != ROOT_PATH ) throw new ArgumentException("APIInfo Root path can only be set once during running of the application");
				_rootPath = value;
			}
		}


		public APIInfoBase (string rootPath = "") {
			if ( rootPath != string.Empty ) _rootPath = rootPath;
		}
	}
}
