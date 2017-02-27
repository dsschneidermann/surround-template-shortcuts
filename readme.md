![VSTSBuild](https://dsschneidermann.visualstudio.com/_apis/public/build/definitions/9c6257ec-b14d-4d32-bb10-b4e149d54fc2/6/badge)
 (currently trying to setup VSTS CI)
 
# Surround Template Shortcuts for ReSharper

This plugin for ReSharper creates keyboard shortcuts to activate Surround Templates.

Surround Templates in ReSharper can be assigned a "mnemonic" number, which is normally only used to show in the surround-with dropdown (Ctrl+E, U). This plugin adds additional keyboard shortcuts that activate Surround Templates on the current text selection.

The shortcut defaults are Ctrl+0 to Ctrl+9 (corresponding to mnemonic numbers 0 to 9) and these can be changed from the Visual Studio keyboard options (search for SurroundTemplateShortcuts).

## Please add feedback on ![UserVoice](./images/UserVoiceLogoWithMargin.png?raw=true)
https://dsschneidermann.uservoice.com

### Features

- Activate Surround Template for current text selection

![Example](./images/SurroundExample.gif?raw=true)

- Activate Surround Template on element before or under cursor

(Currently a bug if cursor is at the end of an element - it won't pick correct.)

- Shortcut to "Move brace to end of selection" (Ctrl+Alt+0)

(May be dropped from final version. Moves the first brace of the text selection to the end. Is intended to make it easier to move auto-inserted braces around.)

### Instructions

- Due to ReSharper limitations, the keyboard bindings must be activated (one-time) from the button in the ReSharper -> Tools menu.

![Example](./images/KeyboardShortcutsButton.png?raw=true)

- Import some premade Surround Templates (ReSharper -> Tools -> Templates Explorer...):

https://github.com/dsschneidermann/surround-template-shortcuts/blob/master/SurroundTemplateShortcuts.DotSettings?raw=true (8 templates)

- Note: you will have *to clear the mnemonic numbers* from any other Surround Templates (or just remove them entirely, if you've never used them like me). Also note the mnemonics have been made with a euro-keyboard where the parenthesis are 8 and 9 - just change if desired.

### Planned

- Support for adding Postfix commands to active Surround Templates
- Improvements to "Activate on element" / "Move brace to end"
- Settings page (if options are needed for the plugin)
