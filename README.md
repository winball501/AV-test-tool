Winball501's Antivirus Test Tool

Winball501's Antivirus Test Tool is a lightweight utility developed specifically for testing antivirus software. It is not an antivirus itself, but a helper tool that makes the testing process easier, more organized, and more efficient.

Features

Error-Based Logging
The tool does not explicitly report whether a file was executed, blocked, or skipped. Instead, it captures and records the error messages from try–catch blocks, giving testers useful insights into how antivirus software reacts during execution attempts.

Progress Tracking with Temporary Files
To avoid repeating samples during testing, the tool creates two temporary files:

executed.txt: Keeps track of how many samples have been opened.

notexecuted.txt: Keeps track of how many samples have not been opened.

⚠️ These files are not logs. They exist only during the test to make sure the tool can resume from where it left off. Once the test is complete, they are automatically deleted.

Resume Functionality
Thanks to the temporary tracking files, the tool can continue exactly from the last tested sample if the process is interrupted.

Purpose

This tool was built for antivirus testing. It helps researchers and developers:
✔ Evaluate antivirus performance against test samples
✔ Save time by avoiding duplicate test runs
✔ In testing system can be restarted or crashed, this tool provide you not lose progress.
<img width="1068" height="585" alt="image" src="https://github.com/user-attachments/assets/bcf4681c-cc10-4141-a8af-011a88a9aa84" />
/>

