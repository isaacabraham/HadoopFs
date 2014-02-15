open HadoopFs.Core
open HadoopFs.Samples

let inputFile = @"outlineofscience.txt"

// A reducer exe
let mainReduce argv =
    doReduce <| SingleValue WordCount.Reducer
    0

[<EntryPoint>]
let mainMap argv =
    doMap <| MultiValue WordCount.Mapper
    0