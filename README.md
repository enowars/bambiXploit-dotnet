# BambiXploit
BambiXploit is ENOFLAG's public exploit parallelization tool.
It will run a given exploit script for every opponent team with staggered starts, gather all flags the exploit writes to stdout, and automatically submit them.
The target's address is passed as a command line parameter.

TL;DR: `bambixploit pwn python3 exploit.py`

## Usage
```
> bambixploit --help
Bambixploit

Usage:
  Bambixploit [options] [command]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  template <Http>                       template command description
  pwn <exploit_command> <exploit_args>  start running the exploit
```

```
> bambixploit pwn --help
pwn
  start running the exploit

Usage:
  Bambixploit [options] pwn <exploit_command> [<exploit_args>...]

Arguments:
  <exploit_command>  the exploit command to run
  <exploit_args>     arguments for the exploit

Options:
  -?, -h, --help  Show help and usage information
```

## Templates
To make developing exploits less time-intensive, BambiXploit can emit exploit templates.
As of now, only a http template exists.

`bambixploit template http` prints the `http` template.

## Installation
 - Download the latest release for your arch
 - Rename the file to `bambixploit`
 - ???
 - PROFIT!

## Configuration
BambiXploit searches for a configuration in the following directories:
 - The current working directory
 - The current user's home directory
 - `/etc/bambixploit/bambixploit.json`
 
A sample configuration can be found [here](https://github.com/enowars/bambiXploit-dotnet/blob/master/bambixploit.json.example).

## Screenshots
![grafik](https://user-images.githubusercontent.com/10261186/139400356-fae13eaa-a69c-4828-9194-1ff9e2c62a79.png)

