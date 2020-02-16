﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core
{
    public static class Constants
    {
        public static string PathToMainFile => Pri.LongPath.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameOfMainFile);
        private static string nameOfMainFile = "testdata.dat";
    }
}