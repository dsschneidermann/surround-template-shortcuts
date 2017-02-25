# Surround Template Shortcuts for ReSharper

This plugin for ReSharper creates keyboard shortcuts to activate Surround Templates.

Surround Templates in ReSharper can be assigned a "mnemonic" number, which is normally only used to show in the surround-with dropdown (Ctrl+E, U). This plugin adds additional keyboard shortcuts that activate Surround Templates on the current text selection.



The shortcut defaults are Ctrl+0 to Ctrl+9 (corresponding to mnemonic numbers 0 to 9) and these can be changed from the Visual Studio keyboard options (search for SurroundTemplateShortcuts).

### Features

- Activate Surround Template for current text selection

![Example](./images/SurroundExample.gif?raw=true)

- Activate Surround Template on element before or under cursor

(GIF is planned)

- Shortcut to "Move brace to end of selection" (Ctrl+Alt+0)

(May be dropped. Moves the first brace of the text selection to the end.)

### Instructions

- Due to ReSharper limitations, the keyboard bindings must be activated (one-time) from the button in the ReSharper -> Tools menu.

![Example](./images/KeyboardShortcutsButton.png?raw=true)

- Import some premade Surround Templates (ReSharper -> Tools -> Templates Explorer...):

https://github.com/dsschneidermann/surround-template-shortcuts/blob/master/SurroundTemplateShortcuts.DotSettings?raw=true (8 templates)

- Note: you will have *to clear the mnemonic numbers* from any other Surround Templates (or just remove them entirely, if you've never used them like me).

### Planned

- Support for adding Postfix commands to active Surround Templates
- Improvements to "Activate on element" / "Move brance to end"
- Settings page (if options are needed for the plugin)
