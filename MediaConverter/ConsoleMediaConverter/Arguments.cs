using System.Linq;

namespace ConsoleMediaConverter
{
    internal class Arguments
    {
        public string FileLocation { get; private set; }
        public string TargetFormat { get; private set; }
        public string StartFormat { get; private set; }
        // settings
        public bool RecursiveFileSearch { get; private set; }
        public bool LogEachFile { get; private set; }
        public bool MultiThreadRequested { get; private set; }
        public bool HelpRequested { get; private set; }


        /// <summary>
        /// Instantiates Arguments object
        /// </summary>
        /// <param name="cmdArgs">commandline arguments</param>
        public Arguments(string[] cmdArgs)
        {
            if (cmdArgs.Contains("-help"))
            {
                HelpRequested = true;
                return;
            }
            else if (cmdArgs.Length < 3)
            {
                throw new InsufficienArgumentsException();
            }
            else
            {
                FileLocation = cmdArgs[0];
                StartFormat = cmdArgs[1];
                TargetFormat = cmdArgs[2];
                if (cmdArgs.Length > 3)
                {
                    var extraParams = new ArraySlice(3, cmdArgs.Length - 1);
                    ParseSettingsArgs(extraParams, cmdArgs);
                }
            }
        }


        /// <summary>
        /// Parses setting arguments
        /// </summary>
        /// <param name="settingSlice">slice containing setting arguments</param>
        /// <param name="args">commandline args</param>
        /// <exception cref="MultipleArgumentException"></exception>
        /// <exception cref="UnknownArgumentException"></exception>
        private void ParseSettingsArgs(ArraySlice settingSlice, string[] args)
        {
            for (int i = settingSlice.Start; i <= settingSlice.End; i++)
            {
                switch (args[i])
                {
                    case "-help":
                        if (HelpRequested) throw new MultipleArgumentException() { Argument = args[i] };
                        HelpRequested = true;
                        break;

                    case "-R": goto case "-r";
                    case "-r":
                        if (RecursiveFileSearch) throw new MultipleArgumentException() { Argument = args[i] };
                        else RecursiveFileSearch = true;
                        break;

                    case "-T": goto case "-t";
                    case "-t":
                        if (MultiThreadRequested) throw new MultipleArgumentException() { Argument = args[i] };
                        else MultiThreadRequested = true;
                        break;

                    case "-l": goto case "-log";
                    case "-log":
                        if (LogEachFile) throw new MultipleArgumentException() { Argument = args[i] };
                        else LogEachFile = true;
                        break;

                    default: throw new UnknownArgumentException() { Argument = args[i] };
                }
            }
        }
    }
}
