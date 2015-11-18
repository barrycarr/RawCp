#RawCp
RawCp is a command line utility that will copy (or move) RAW files from you camera's memory card to a location of your choice.

Your RAW files are copied (or moved) to a location under your specified destination folder based on the date of the raw file,
a description that you supply on the comand line and the camera name that you also supply at the command line. A default source location and the default root destination folder can be specified the utilities config file (`rawcp-config.json`).

###Config File
RawCp's config file is a JSON file that allows you to specify some default values that RawCp uses each time it is run. RawCp will look for a config file in one of three locations and expects the config file to be called `rawcp-config.json`

  1. The current directory
  2. The directory where RawCp resides
  3. Your home directory

RawCp will look for the config file in each of these locations in the order listed, above.
 
The following table describes each of the values that the config file holds:

Property     | Type | Purpose | Example
-------------|----------| -------|----------------------------------------------------------
`"alwaysMove"` | `Boolean` | Tells RawCp to always move files rather than copy them. It's the equivalent of invoking `RawCp` with the `/m` switch. | `"alwaysMove": true`
`"cameras"` | `Object` (More specifically, a `Dictionary<string, string>`) | An object that consists of key-value pairs. Each key-value pair is defined thus `"key": "value"`. Providing `RawCp` with a key on the command line, via `/c` switch, provides you with a means of giving RawCp an alias for a cameras full description. | `"cameras": {"a7": "Sony A7", "nex5": "Sony NEX-5N","nex6": "Sony NEX-6","x100s": "Fuji x100s"}`
`"defaultCamera"`|`String`| A string that matches one of the keys defined in `"camera"` property. Used as the default value when you don't use the `/c cameraKey` on the command line. | `"defaultCamera": "x100s"` 
`"destinationFolder"`|`String`| The location (path) where you'd like your RAW files copied to. Please note that backslashes in the path have to be doubled up. | `"destinationFolder": "c:\\Users\\Username\\Pictures"`
`"sourceFolder"` | `String` | The location (path) where your raw files are located. Please note that backslashes in the path have to be doubled up. | `"sourceFolder": "z:\\DCIM"`

#### An Example Config File
Here is a complete example config file based on the examples used in the table above.
```
{
    "alwaysMove": true,
    "cameras": {
        "a7": "Sony A7",
        "nex5": "Sony NEX-5N",
        "nex6": "Sony NEX-6",
        "x100s": "Fuji x100s"
    },
    "defaultCamera": "x100s",
    "destinationFolder": "c:\\test\\dest",
    "sourceFolder": "c:\\test\\src"
}
```

###Usage
```
RawCp /d "description" /c "camera" /m
```
