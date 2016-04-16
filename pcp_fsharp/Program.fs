open RawCp.Config
open RawCp.Params
open RawCp.CommandLineOptions
open RawCp.FileHandling
open RawCp.Either

let printLegend opts (conf: Config)  dest =
    let mthd = if opts.Move
                  then "MOVING" 
                  else "COPYING"
    printfn "RawCp: v0.1"
    printfn "  %s files" mthd
    printfn "  from: %s" conf.SourceFolder
    printfn "  to:   %s" dest;


let getConfig configFilename p =
    let config = Helpers.load configFilename
    match config with
    | Failure c -> failure c
    | Success c ->
        success {p with Config = c} 


let getOptions args p =
    let opts = RawCp.CommandLineParser.parse args p.Config
    match opts with
    | Failure x -> failure x
    | Success o -> success {p with Opts = o}

        
let copy p =
    let destFolder = destinationFolder p 

    printLegend p.Opts p.Config destFolder

    let results = copyFiles p.Filenames destFolder
    results.Wait()

    deleteSourceFiles p.Opts.Move p.Filenames
    Success 0 // return an integer exit code    

                                       
[<EntryPoint>]
let main argv =
    let x = getConfig "rawcp-config.json" RawCp.Params.empty
        >>= getOptions argv
        >>= getFilenames
        >>= copy
    
    match x with
    | Success n -> n
    | Failure m -> 
        printfn "%s" m    
        -1
