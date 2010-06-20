// 
// Copyright (c) 2004-2010 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace NLog.UnitTests.Targets
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    using System.Threading;
    using NLog.Internal;
    using System.Collections.Generic;

    [TestClass]
    public class ConsoleTargetTests : NLogTestBase
    {
        [TestMethod]
        public void ConsoleOutTest()
        {
            var target = new ConsoleTarget()
            {
                Header = "-- header --",
                Layout = "${logger} ${message}",
                Footer = "-- footer --",
            };

            var consoleOutWriter = new StringWriter();
            TextWriter oldConsoleOutWriter = Console.Out;
            Console.SetOut(consoleOutWriter);

            try
            {
                var exceptions = new List<Exception>();
                target.Initialize();
                target.WriteAsyncLogEvent(new LogEventInfo(LogLevel.Info, "Logger1", "message1").WithContinuation(exceptions.Add));
                target.WriteAsyncLogEvent(new LogEventInfo(LogLevel.Info, "Logger1", "message2").WithContinuation(exceptions.Add));
                target.WriteAsyncLogEvents(
                    new LogEventInfo(LogLevel.Info, "Logger1", "message3").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger2", "message4").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger2", "message5").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger1", "message6").WithContinuation(exceptions.Add));
                Assert.AreEqual(6, exceptions.Count);
                target.Close();
            }
            finally 
            {
                Console.SetOut(oldConsoleOutWriter);
            }

            string expectedResult = @"-- header --
Logger1 message1
Logger1 message2
Logger1 message3
Logger2 message4
Logger2 message5
Logger1 message6
-- footer --
";
            Assert.AreEqual(expectedResult, consoleOutWriter.ToString());
        }

#if !NET_CF
        [TestMethod]
        public void ConsoleErrorTest()
        {
            var target = new ConsoleTarget()
            {
                Header = "-- header --",
                Layout = "${logger} ${message}",
                Footer = "-- footer --",
                Error = true,
            };

            var consoleErrorWriter = new StringWriter();
            TextWriter oldConsoleErrorWriter = Console.Error;
            Console.SetError(consoleErrorWriter);

            try
            {
                var exceptions = new List<Exception>();
                target.Initialize();
                target.WriteAsyncLogEvent(new LogEventInfo(LogLevel.Info, "Logger1", "message1").WithContinuation(exceptions.Add));
                target.WriteAsyncLogEvent(new LogEventInfo(LogLevel.Info, "Logger1", "message2").WithContinuation(exceptions.Add));
                target.WriteAsyncLogEvents(
                    new LogEventInfo(LogLevel.Info, "Logger1", "message3").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger2", "message4").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger2", "message5").WithContinuation(exceptions.Add),
                    new LogEventInfo(LogLevel.Info, "Logger1", "message6").WithContinuation(exceptions.Add));
                Assert.AreEqual(6, exceptions.Count);
                target.Close();
            }
            finally
            {
                Console.SetError(oldConsoleErrorWriter);
            }

            string expectedResult = @"-- header --
Logger1 message1
Logger1 message2
Logger1 message3
Logger2 message4
Logger2 message5
Logger1 message6
-- footer --
";
            Assert.AreEqual(expectedResult, consoleErrorWriter.ToString());
        }
#endif
    }
}
