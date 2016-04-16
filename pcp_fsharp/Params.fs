module RawCp.Params

open System

open RawCp.Config
open RawCp.CommandLineOptions

type Params = {
    Config: Config; 
    Opts: CommandLineOptions;
    Date: DateTime;
    Filenames: seq<String>;
}

let empty = {
    Config = Config();
    Opts = RawCp.CommandLineOptions.empty;
    Date = DateTime.MinValue;
    Filenames = Seq.empty<String>
}

