namespace RawCp

type Result<'TEntity> =
    | Success of 'TEntity
    | Failure of string

module Either =

    let bind switchFunction twoTrackInput =
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> Failure f

    let map singleTrackFunction =
        bind (singleTrackFunction >> Success)