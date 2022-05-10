# DiscoShell

DiscoShell (a.k.a. DiscoSh) is a minimal remote access trojan that is controlled via a Discord bot, allowing the creation of a Discord-based botnet.

Support for the builder has been officially dropped.

![image](https://media.discordapp.net/attachments/961905736139554876/971733014654644254/unknown.png)

Join the Discord server for discussion and enquiries: https://discord.gg/RU5RjSe8WN.

## Detection status
Virustotal result:
![image](https://media.discordapp.net/attachments/961905736139554876/972065831083200512/unknown.png)
[Link](https://www.virustotal.com/gui/file/de8d1c39d6289ab2c345f82f75c4c1c7dcb5f2e202d5a8100ac97bb921559035/detection)

Also tested on a VM against Windows Defender, Avast and AVG.

None of the above guarantee that it is fully undetectable.

## Features
- Persistence
- UAC bypass
- Fileless

\+ Can perform all things described in the [Discord bot commands](#discord-bot-commands) section.

## Discord bot commands
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

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
