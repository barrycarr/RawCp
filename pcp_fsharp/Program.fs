// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

// http://www.codeproject.com/Articles/773451/Fsharp-Asynchronous-Workflows

open Newtonsoft.Json
open Config
open System
open System.IO
open System.Threading.Tasks

let load filename =
     JsonConvert.DeserializeObject<Config.Config>(File.ReadAllText(filename))

let filelist dir =
    let dirInfo = DirectoryInfo(dir)
    let files = dirInfo.GetFiles("*.*") |> Array.sortBy (fun fi -> fi.CreationTime);
    
    Directory.GetFiles(dir)

let copyFiles files dest =
    let asyncAwaitVoidTask (t: Task) =
        Async.AwaitIAsyncResult(t) |> Async.Ignore

    let copyFile dest f =
        async {
            let d = Path.Combine(dest, Path.GetFileName(f))
            use reader = File.Open(f, FileMode.Open)
            use writer = File.Create(d)
            
            printfn "Copying: %s -> %s" f d
            
            do! reader.CopyToAsync(writer) |> asyncAwaitVoidTask
            do! writer.FlushAsync() |> asyncAwaitVoidTask
        } |> Async.StartAsTask
            
    Task.WhenAll(files |> Array.map (copyFile dest)) 
    

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    let config = load "./pcp-config.json"
    let files = filelist config.SourceFolder
    let results = copyFiles files config.DestinationFolder
    results.Wait()

    0 // return an integer exit code
