namespace RawCp

// http://fsharpforfunandprofit.com/posts/pattern-matching-command-line/
module CommandLineParser =
    open RawCp.Config
    open RawCp.Either
    open RawCp.CommandLineOptions

    let parse args (config: Config) =
        let rec parseRec args options =
            match options with
            | Failure s -> failure s
            | Success optionsSoFar ->
                match args with
                | [] -> success optionsSoFar
                | "/d"::xs ->
                    let newOpts = { optionsSoFar with Description = xs.Head }
                    parseRec xs.Tail (success newOpts)
                | "/c"::xs ->
                    let newOpts = {optionsSoFar with Camera = xs.Head }
                    parseRec xs.Tail (success newOpts)
                | "/m"::xs ->
                    let newOpts = {optionsSoFar with Move = true }
                    parseRec xs.Tail (success newOpts)
                | x::_ -> failure (sprintf "Option %s is unrecognised" x)
        
        let defaultOpts = {
            Description = "";
            Camera = config.DefaultCamera;
            Move = config.AlwaysMove
        }
        
        parseRec (args |> Array.toList) (success defaultOpts)
        
        
