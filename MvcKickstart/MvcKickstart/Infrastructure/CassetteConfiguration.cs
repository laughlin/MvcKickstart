using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace MvcKickstart.Infrastructure
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
		public void Configure(BundleCollection bundles)
		{
			bundles.AddPerIndividualFile<StylesheetBundle>("Content/css", new ExcludeDirectorySearch("*.css", new[] { "_lib" }));
			bundles.AddPerSubDirectory<StylesheetBundle>("Content/css/_lib");

			bundles.AddPerIndividualFile<StylesheetBundle>("Content/less", new ExcludeDirectorySearch("*.less", new[] { "_lib", "Shared" }));
			bundles.Add<StylesheetBundle>("Content/less/Shared");
			bundles.AddPerSubDirectory<StylesheetBundle>("Content/less/_lib");

			// Exclude the lib and Admin directories since those are bundled per directory
			bundles.AddPerIndividualFile<ScriptBundle>("Content/js", new ExcludeDirectorySearch("*.js", new []{"_lib"}));
			bundles.AddPerSubDirectory<ScriptBundle>("Content/js/_lib");

//			bundles.AddUrlWithAlias<ScriptBundle>(VirtualPathUtility.ToAbsolute("~/signalr/hubs"), "Content/js/_lib/signalr-hubs", b => b.AddReference("~/Content/js/_lib/signalr"));
		}

		/// <summary>
		/// An exclude directory search for Cassette. Provide the patterns you want to search for
		/// and this will exclude *.min/*-vsdoc files as well as the directories you specify.
		/// </summary>
		/// <see cref="http://kamranicus.com/Blog/Posts/27/using-cassette-for-semi-complicated-scenarios-upda"/>
		internal class ExcludeDirectorySearch : FileSearch
		{
			/// <summary>
			/// Excludes specified directories in search (also .min and -vsdoc files)
			/// </summary>
			/// <param name="pattern">File search pattern (wildcards, e.g. *.css;*.less)</param>
			/// <param name="directoriesToExclude">A string array of folder names to exclude. Will match anywhere in full file path.</param>
			public ExcludeDirectorySearch(string pattern, string[] directoriesToExclude)
			{
				SearchOption = SearchOption.AllDirectories;
				Pattern = pattern;
				ExcludeDirectories = directoriesToExclude;
			}

			private string[] ExcludeDirectories
			{
				set
				{
					// Join with rx | (or)
					var directories = String.Join("|", value);

					// Extensions "*.js;*.coffee" => "js", "coffee"
					// Assumes you're using wildcard... but whatever.
					var extensions = String.Join("|", Pattern.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).
					Select(s => s.Substring(2)).ToArray());

					var excludeRegex =
					new Regex(String.Format(@"({0})[\\/]|([\.-](min)\.({1})$)", directories, extensions), RegexOptions.IgnorePatternWhitespace);

					// Set exclusions
					Exclude = excludeRegex;
				}
			}
		}
	}
}