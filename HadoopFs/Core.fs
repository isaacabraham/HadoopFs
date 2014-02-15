module HadoopFs.Core

open HadoopFs.IO
open System
open System.IO

module internal Helpers = 
    /// Generates a stream of key/values grouped by adjacent key from a sequence of ordered key/value pairs.
    let getStream (data : seq<_>) = 
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
        parts.[0], parts.[1]
    
    /// Converts the input function into a stream of lines.
    let makeSequence getNextRow = 
        seq { 
            let line = ref (getNextRow())
            while (!line <> null) do
                yield !line
                line := getNextRow()
        }

let private processToOutput (action, outputRow) = 
    Seq.collect action >> Seq.iter (fun (key, value) -> outputRow <| sprintf "%s\t%s" key value)

/// Runs a mapper using a custom Reader and Writer
let doMapCustom (mapper, getNextRow, outputRow) = 
    getNextRow
    |> Helpers.makeSequence
    |> processToOutput (mapper, outputRow)

/// Runs a reducer using a custom Reader and Writer
let doReduceCustom (reducer, (getNextRow : unit -> string), outputRow) = 
    getNextRow
    |> Helpers.makeSequence
    |> Seq.map Helpers.intoKeyValue
    |> Helpers.getStream
    |> processToOutput (reducer, outputRow)

/// Runs a mapper using the Console for IO, ideal for use with Streaming Hadoop.
let doMap mapper = doMapCustom (mapper, Readers.Console, Writers.Console)

/// Runs a reducer using the Console for IO, ideal for use with Streaming Hadoop.
let doReduce reducer = doReduceCustom (reducer, Readers.Console, Writers.Console)