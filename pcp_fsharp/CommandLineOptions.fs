module RawCp.CommandLineOptions

type CommandLineOptions = {
    Description: string;
    Camera: string;
    Move: bool
}

let empty = {
    Description = "";
    Camera = "";
    Move = false;
}

