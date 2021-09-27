﻿using System;
using System.IO;

namespace SlaamMono.Helpers.Logging
{
    public class TextFileLog : ILoggingDevice
    {
        private const string DefaultFileName = "log.log";

        private TextWriter _textWriter;


        public void Begin()
        {
            _textWriter = File.CreateText(DefaultFileName);
        }
        public void Log(string line)
        {
            _textWriter.WriteLine(line);
        }

        public void End()
        {
            _textWriter.Close();
        }
    }
}
