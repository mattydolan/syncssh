using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace emri_repository_tests
{
    public class TestHelper
    {
        private static IConfigurationRoot _localCache;
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            if(_localCache != null)
            {
                return _localCache;
            }

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.test.json", optional: false)           
                .Build();

            _localCache = configRoot;
            return configRoot;
        }

        public static IConfigurationRoot GetIConfigurationRoot()
        {
            if (_localCache != null)
            {
                return _localCache;
            }

            Assembly asm = Assembly.GetExecutingAssembly();

            string testAssemblyPath = asm.Location;
            string testDirectory = testAssemblyPath.Replace(asm.ManifestModule.Name, string.Empty);


            return GetIConfigurationRoot(testDirectory);
        }
    }
}
