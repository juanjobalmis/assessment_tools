# Assessment Tools

* You have to install the **.NET 6** runtime.
* Tool to be used not only with **Moodle** but also with other similar assessment methods.

* **How to use the program?**
    1. Set the **PATH** environment variable to where **assesst.exe** and dependencies were located.
    2. Unzip received students assignments whether they are downloaded from Moodle. Otherwise, each assignment has to be in a folder with their name.
    3. Copy the rubric template file to the assignments folders.
    4. Open a command line terminal in the assignments folder.
    5. Options:

        ```
        assesst Uses ...

                assesst <command>

                <command>
                        RG (RUBRIC_GENERATOR):
                                 * To generate rubrics from an assessment template.
                        AG (ASSESSMENT_GENERATOR):
                                 * To generate an assessment report from a rubrics folder.
        Type for more information: assesst <command> --help
        ```

---

## Commands

### RUBRIC_GENERATOR

* assesst RUBRIC_GENERATOR Uses ...

    ```txt
        assesst RG --help
        assesst RG <options>

        <options>
                -v (--verbose):
                         * To give feedback of the process.
                -s (--studentNamesFile) <studentNamesFile>:
                         * Is a CSV file with ';' separator.
                         * The following name colums are allowed: Name, ID, NickName, GitHubLogin, Mail, Group.
                                 - Name (required): Student name without accentuation symbols.
                                 - ID (required): Identifies the student and it has to be unique.
                                   The ID number will help us as deliver's alias and for assessing Moodle assignments.
                                 - NickName (optinal): Possible student alias.
                                 - GitHubLogin (optinal): Possible student alias and to download and map GitHub roaster.
                                 - Mail (optinal): To give feedback if you don't use any VLE environment.
                                 - Group (optinal): To classify students in reports.
                -n (--normalized):
                         * Normalize the character to Unicode C.
                         * Uppercase conversion.
                         * Reduces the folder name to the student name. (whether it is possible).
                         * Not necessary if you define the --studentNamesFile.
                -r (--rubricTemplateFile) <rubricTemplateFile>
                         * To specify the file name with the assessment rubric template.

                 Note: The columns ID, NickName and GitHubLogin are used as possible
                       students alias to match student name in the delivered folders.
                       In case of ambiguity, the program tries to match an existing
                       folder with a student name, asking us for confirmation.
                       Besides, use students name file implies normalization.
    ```

* **Example:** `assesst RG -v -s studentsData.csv -r assessmentTemplate.xlsx`

---

### ASSESSMENT_GENERATOR

* assesst ASSESSMENT_GENERATOR Uses ...

    ```txt
        assesst AG --help
        assesst AG  <options>

        <options>
                -v (--verbose):
                         * To give feedback of the process.
                -s (--studentNamesFile) <studentNamesFile>:
                         * Is a CSV file with ';' separator.
                         * The following name colums are allowed: Name, ID, NickName, GitHubLogin, Mail, Group.
                                 - Name (required): Student name without accentuation symbols.
                                 - ID (required): Identifies the student and it has to be unique.
                                   The ID number will help us as deliver's alias and for assessing Moodle assignments.
                                 - NickName (optinal): Possible student alias.
                                 - GitHubLogin (optinal): Possible student alias and to download and map GitHub roaster.
                                 - Mail (optinal): To give feedback if you don't use any VLE environment.
                                 - Group (optinal): To classify students in reports.
                -n (--assignmentName) <name>:
                         * Name of the asignament.
                         * If it is not specified the current folder name is taken.
                -t (--targetPlatform) <target>:
                         * -t moodlexml -> It generates a moodle calification format into moodle.xml file.
                                           To set the assignment Name and the student ID see Moodle doc at
                                           https://docs.moodle.org/38/en/Grade_import#XML_import
                         * -t moodlecsv -> It generates a CSV calification format into moodle.csv file.
                                           We need to map Email with ID to identify a Student see
                                           https://docs.moodle.org/38/en/Grade_import#CSV_import
                         * -t gmail -> It generates crypted credentials for SMTP server whether there are not.
                                       Then it will send a e-Mail to each estudent with their rubric information
                                       The message subject is the assignmentName value.
                Note: The xmlx book file with the assessment has to have a Named cell or range
                called 'mark' to find out the assessment mark or score in some targets.
    ```

* **Example:** `assesst AG -v -s studentsData.csv -n exercise1 -t moodlcsv`

---