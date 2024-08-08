namespace RawCp

// http://fsharpforfunandprofit.com/posts/pattern-matching-command-line/
module CommandLine =
    open RawCp.Config

    type CommandLineOptions = {
        Description: string;
        Camera: string;
        Move: bool
    }

    let parseCommandLine args (config: Config) =
        let rec parseCommandLineRec args optionsSoFar =
            match args with
            | [] -> optionsSoFar
            | "/d"::xs ->
                let newOpts = { optionsSoFar with Description = xs.Head }
                parseCommandLineRec xs.Tail newOpts
            | "/c"::xs ->
                let newOpts = {optionsSoFar with Camera = xs.Head }
                parseCommandLineRec xs.Tail newOpts
            | "/m"::xs ->
                let newOpts = {optionsSoFar with Move = true }
                parseCommandLineRec xs.Tail newOpts
            | x::xs ->
                printfn "Option %s is unrecognised" x
                parseCommandLineRec xs optionsSoFar

        let defaultOpts = {
            Description = "";
            Camera = config.DefaultCamera;
            Move = config.AlwaysMove
        }
        
        parseCommandLineRec (args |> Array.toList) defaultOpts 
