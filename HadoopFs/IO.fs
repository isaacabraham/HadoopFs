namespace HadoopFs.IO

open System
open System.IO

/// Provides a set of readers to use within streaming map tasks.
module Readers = 
    /// Reads from the Console.
    let Console = Console.ReadLine
    
    /// Reads from a file.
    let FileSystem(path : string) = (new StreamReader(path)).ReadLine
    
    /// Reads from an in-memory list.
    let Collection(lines) = 
        let enumerator = (lines |> Seq.ofList).GetEnumerator()
        fun () -> 
            if enumerator.MoveNext() then enumerator.Current
            else null

/// Provides a set of writers to use within streaming reduce tasks.
module Writers = 
    /// Writes to the Console
    let Console(line : string) = Console.WriteLine(line)
    
    /// Writes to a file
    let FileSystem(path : string) = 
        fun (line : string) -> 
            use writer = new StreamWriter(path, true)
            writer.WriteLine(line)