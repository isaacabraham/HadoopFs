module HadoopFs.Core

open HadoopFs.IO
open System
open System.IO

module internal Helpers = 
    /// Generates a stream of key/values grouped by adjacent key from a sequence of ordered key/value pairs.
    let groupByAdjacent (data : seq<_>) = 
        let lastKey = ref String.Empty
        let enumerator = data.GetEnumerator()
        seq { 
            let moreData = ref (enumerator.MoveNext())
            while !moreData do
                let key, value = enumerator.Current
                if !lastKey <> key then 
                    lastKey := key
                    yield key, 
                          seq { 
                              while !moreData && (!lastKey = (fst enumerator.Current)) do
                                  yield enumerator.Current |> snd
                                  moreData := enumerator.MoveNext()
                          }
        }
    
    /// Create a key/value pair from a string.
    let intoKeyValue (line : string) = 
        let parts = line.Split('\t')
        match parts with
        | [| key; value |] -> key,value
        | _ -> failwith "line should only have two parts when split on tab"

/// Represents the different types of Map or Reducer.
type MRFunction<'a, 'b, 'c> = 
    /// A mapper or reducer that contains zero or one output.
    | SingleOutput of ('a -> ('c * 'b) option)
    /// A mapper or reducer that contains many outputs.
    | ManyOutputs of ('a -> seq<'c * 'b>)

let private processToOutput (action, writer) = 
    match action with
    | SingleOutput action -> Seq.choose action
    | ManyOutputs action -> Seq.collect action
    >> Seq.map (fun (key, value) -> sprintf "%s\t%s" (key.ToString()) (value.ToString()))
    >> Seq.iter writer

/// Runs a mapper using a custom Reader and Writer.
let doMapCustom mapper writer reader = reader |> processToOutput (mapper, writer)

/// Runs a reducer using a custom Reader and Writer.
let doReduceCustom reducer writer reader = 
    reader
    |> Seq.map Helpers.intoKeyValue
    |> Helpers.groupByAdjacent
    |> processToOutput (reducer, writer)

/// Runs a mapper using the Console for IO, ideal for use with Streaming Hadoop.
let doMap mapper = doMapCustom mapper Writers.Console Readers.Console

/// Runs a reducer using the Console for IO, ideal for use with Streaming Hadoop.
let doReduce reducer = doReduceCustom reducer Writers.Console Readers.Console