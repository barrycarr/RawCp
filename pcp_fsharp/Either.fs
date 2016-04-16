module RawCp.Either

type Result<'TEntity> =
    | Success of 'TEntity
    | Failure of string

let success x =
    Success x

let failure s =
    Failure s

let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f

let (>>=) twoTrackInput switchFunction =
    bind switchFunction twoTrackInput

let map singleTrackFunction =
    bind (singleTrackFunction >> Success)

let returnMessage result =
    match result with
    | Success obj -> obj.ToString
    | Failure msg -> msg.ToString