namespace RawCp

// http://fsharpforfunandprofit.com/posts/pattern-matching-command-line/
module CommandLine =
    open RawCp.Config
    open RawCp.Either

    type CommandLineOptions = {
        Description: string;
        Camera: string;
        Move: bool
    }

    let parseCommandLine args (config: Result<Config>) =
        let rec parseCommandLineRec args options =
            match options with
            | Failure s -> failure s
            | Success optionsSoFar ->
                match args with
                | [] -> success optionsSoFar
                | "/d"::xs ->
                    let newOpts = { optionsSoFar with Description = xs.Head }
                    parseCommandLineRec xs.Tail (success newOpts)
                | "/c"::xs ->
                    let newOpts = {optionsSoFar with Camera = xs.Head }
                    parseCommandLineRec xs.Tail (success newOpts)
                | "/m"::xs ->
                    let newOpts = {optionsSoFar with Move = true }
                    parseCommandLineRec xs.Tail (success newOpts)
                | x::xs ->
                    failure (sprintf "Option %s is unrecognised" x)

        match config with
        | Success c -> 
            let defaultOpts = {
                Description = "";
                Camera = c.DefaultCamera;
                Move = c.AlwaysMove
            }
        
            parseCommandLineRec (args |> Array.toList) (success defaultOpts)
        
        | Failure s -> failure s     
