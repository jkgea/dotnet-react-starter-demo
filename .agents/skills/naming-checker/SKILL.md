---
name: naming-checker
description: 'Validate file, variable, constant, and type naming conventions against project standards. Use whenever the user asks to check, audit, review, or fix names or naming conventions.'
allowed-tools: shell
---

# Naming Checker

Audit names against the standards that govern the selected project scope and report precise, low-noise fixes.

## Workflow

1. Determine the requested scope. Use the repository root when the user does not specify one.
2. Read the naming rules that apply to that scope, in this priority order:
   - repository instruction files such as `AGENTS.md` and `.github/copilot-instructions.md`
   - formatter, compiler, and linter configuration
   - project documentation and framework conventions
   - consistent patterns in nearby source and test files
3. Run [`scripts/check-file-names.sh`](./scripts/check-file-names.sh) with the scope as its argument. The script reports lowercase-leading camelCase filenames that should be kebab-case and excludes `.agent.md` and filenames containing `.test.`.
4. Inspect source symbols using the project's language-aware search, compiler, or linter tools when available. Check:
   - files and directories against documented layer and framework patterns
   - variables, parameters, fields, and properties
   - constants and enum members
   - classes, interfaces, types, records, and type aliases
5. Prefer explicit project rules over generic language conventions. When no explicit rule exists, use established language defaults:
   - TypeScript/JavaScript variables use `camelCase`, constants use the locally established constant style, and types/components use `PascalCase`
   - C# locals and parameters use `camelCase`, private fields use `_camelCase`, types and public members use `PascalCase`, and interfaces use `IPascalCase`
6. Do not flag generated output, vendored dependencies, build artifacts, or names required by a framework or external API.
7. Deduplicate script and source-review findings. Assign severity based on impact:
   - **Error**: violates an enforced compiler, linter, serialization, or externally visible contract
   - **Warning**: violates an explicit project standard
   - **Info**: differs from a consistent local convention that is not explicitly enforced
8. Suggest the exact replacement name. Mention reference updates or a case-safe rename when relevant, but do not rename anything unless the user asks.

## File Scanner

Run from any directory:

```bash
bash .agents/skills/naming-checker/scripts/check-file-names.sh [path ...]
```

If the skill path is not relative to the current directory, invoke the script by its resolved absolute path. Pass one or more files or directories; omitted paths default to the current directory.

## Output Format

Return one combined Markdown table:

| Severity | Category | Location | Current name | Rule | Suggested fix |
|---|---|---|---|---|---|
| Warning | File | `src/exampleFile.ts:1` | `exampleFile.ts` | Source files use kebab-case | Rename to `example-file.ts` and update imports |

Use file paths and line numbers for source symbols. Use line `1` for filename findings. Sort findings by severity, then location. If there are no findings, return the table headers followed by: `No naming convention violations found.`

After the table, briefly list which standards files and automated tools were consulted. Distinguish verified project rules from fallback conventions when that distinction affects a finding.
