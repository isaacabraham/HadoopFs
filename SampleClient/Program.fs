open HadoopFs.Core
open HadoopFs.IO
open HadoopFs.Samples
open HadoopFs.Testability

//let mapAndReducer = (ManyOutputs WordCount.Mapper, SingleOutput WordCount.Reducer) ||> doMapReduce
//let consoleIo = (Writers.Console, Readers.TestableConsole)

// A test map reduce exe
//let mainMapReduce argv = 
//    consoleIo ||> mapAndReducer 
//    0

// A mapper exe
//let mainMap argv = 
//    doMap <| ManyOutputs WordCount.Mapper
//    0

[<EntryPoint>]
// A reducer exe
let mainReduce argv = 
    doReduce <| SingleOutput WordCount.Reducer
    0