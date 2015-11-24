#RawCp
RawCp is a command line utility that will copy (or move) RAW files from you camera's memory card to a location of your choice.

Your RAW files are copied (or moved) to a location under your specified destination folder based on the date of the raw file,
a description that you supply on the comand line and the camera name that you also supply at the command line. A default source location and the default root destination folder can be specified the utilities config file (`rawcp-config.json`).

*Please note: This is beta quality software and a work in progress, as such, it is provided as-is. You should use this sofware with caution and back-up any important data before you this utility to copy or move any of data/images files.*
  
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
    "destinationFolder": "c:\\Users\\Username\\Pictures",
    "sourceFolder": "z:\\DCIM"
}
```

### Usage
```
RawCp /d "descriptiveDirectoryName" /c "cameraKey" /m
```
Where:
  `/d "descriptiveDirectoryName"` is the name of the of sub directory where your files will be copied to below the `destinationFolder` specified in your config file (see the example below).

  `/c "cameraKey"` is one of the camera keys defined in your config file. A sub-folder will created, below the folder specified by the `/d` parameter named after the `value` the provided `cameraKey` relates to (see the example below).

  `/m` specifies that you want your files moved and not just copied. By moved, I mean that once copied they will deleted from the location specified `sourceFolder` property in the config file.

### Example
In this example, we're going to assume that went on a photo outing on the 5th August 2015 (all the RAW files on your memory card will have this date) and that you used two cameras, Your Sony A7 and your Fuji x100s. We're also going to assume, that your config file is defined as above. Finally, we're going to assume that you've put RawCp somewhere on your path and your config is located in your home directory.

We'll deal with RAW files on the Sony A7 first. Once you've connected your camera to your PC, or inserted the A7's memory card into your card reader (drive Z:), you then open up a command line terminal and enter the following command:

```
RawCp /d "Day Trip to Dundee" /c a7
```
This command will:
  Copy all the files on your memory card located at `z:\DCIM` (so, if you shoot RAW+JPEG, the JPEGs will also be copied) to:
  ```
  c:\Users\Username\Pictures\2015\08 August\20151105 Day Trip to Dundee\Sony A7
  ```
  (I'll explain how the above path was generated below) Finally, once all of the files have been copied from `z:\DCIM` they will be deleted because `alwaysMove` is set to `true` in your config file.

  Next, you connect your Fuji x100s to your PC or, insert it's memory card into your reader and enter the following:
 ```
 RawCp /d "Day Trip to Dundee"
 ```
 Because you haven't supplied a `/c`, `RawCp` uses the `defaultCamera` value specified in the config file. This will copy all the files from the memory card to:
 ```
 c:\Users\Username\Pictures\2015\08 August\20151105 Day Trip to Dundee\Fuji x100s
 ```
#### How destination paths are generated
Here is the destination path from the first part of the example. This time, I've numbered each part of the path.
 ```
c:\Users\Username\Pictures\2015\08 August\20151105 Day Trip to Dundee\Sony A7
|           1             | 2  |    3    |            4              |   5  |
  ```
1. Is determined by the value specified by the `destinationFolder` value in the config file.
2. Is the year taken from the RAW files date
3. Is the months number and months name taken from the RAW files date. Month number and dates are used so that all the months appear in the correct order in that year's folder. All months numbers are two digits long, leading zeros are used for the months before October.
4. Is the RAW file's date in ISO format (YYYYMMDD) followed by the text you gave the `/d` switch. As with month numbers above, ISO dates are used to ensure that directories appear in the order in which the photos were taken.
5. This is the value associated with key provided with `/c` switch. In our example, the `a7` key maps to the value `Sony A7` in the config file. If the `/c` isn't supplied, the value that maps key specified by the `defaultCamera` property of the config file is used.

### Other Points
If `RawCp` finds that there are RAW files that have different dates in `sourceFolder` then it will only copy the earliest dated files. If you've elected to move the files, you can keep using `RawCp` until the `sourceFolder` is empty and supply a different description (`/d` switch) each time.

It should be possible to place a copy of your config file in the root folder of each memory card. This config file can then be edited to contain the default values for that particular card/camera combination. Then, before you run `RawCp`, `cd` into the card's root directory and `RawCp` should pick up the config file located there and all you will need to do is provide `RawCp` a description via the `/d` switch.
