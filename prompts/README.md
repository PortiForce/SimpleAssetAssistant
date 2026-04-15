# Prompts

Task-specific prompt recipes for Copilot and AI coding agents.

## How these relate to other guidance files

| File | Purpose |
|---|---|
| `AGENTS.md` | Full authoritative spec: rule priority, rationale, workflow, known repo realities |
| `.github/copilot-instructions.md` | Short, always-on operational rules Copilot reads every time |
| `prompts/*.md` | One-shot task checklists — they reference the rules, they do not restate them |

**Do not duplicate** rules that already live in `copilot-instructions.md` or `AGENTS.md`.
Prompts should add only the task-specific steps, paths, and reminders that are not in the global rules.

## Standard prompt template

Every prompt file should use this structure:

```markdown
# <Task title>

## Goal
One-sentence description of what this task produces.

## Where
Paths and layers where changes belong.

## Steps
Ordered checklist of what to do.

## Don'ts
Common mistakes to avoid for this specific task.

## Definition of Done
How to verify the task is complete (tests, build, review).

## Questions
What to ask the user when context is ambiguous.
```

## Adding a new prompt

1. Copy the template above into `prompts/<task-name>.md`.
2. Fill in the sections with task-specific guidance.
3. Reference layer paths from `AGENTS.md` — do not invent new ones.
4. Keep the prompt short and actionable.
