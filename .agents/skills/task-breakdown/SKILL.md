---
name: task-breakdown
description: 'Break down, decompose, or plan implementation for feature requests and user stories into implementable sub-tasks with T-shirt size estimates, dependencies, acceptance criteria, and risk assessment.'
argument-hint: 'Feature request or user story to break down'
---

# Task Breakdown

Turn a feature request or user story into an ordered implementation plan whose tasks can be assigned, built, and verified independently.

## When to Use

Use this skill when the user asks to:

- "break down" a feature or user story
- "decompose" a requirement
- "plan implementation" for a feature

## Workflow

1. Read the request and available repository context. Identify the user outcome, affected components, constraints, and non-functional requirements.
2. Ask a concise clarifying question only when a missing answer would materially change the plan. Otherwise, state necessary assumptions.
3. Split the work into the smallest independently implementable and verifiable tasks. Include relevant discovery, schema, backend, frontend, testing, documentation, rollout, or observability work without inventing unnecessary layers.
4. Order tasks by dependency. Use task titles in the Dependencies column; use `None` for tasks that can start immediately.
5. Assign one T-shirt size to each task using the rubric below. Size the implementation effort and uncertainty together, not elapsed calendar time.
6. Check every task for risk. A risk assessment is mandatory when a task touches a shared code path or requires a database migration.
7. Verify that every acceptance criterion is observable, testable, and scoped to its task.

## T-Shirt Size Rubric

| Size | Typical scope |
|---|---|
| XS | Trivial, localized change with a known approach and minimal testing |
| S | Small change in one component with straightforward tests |
| M | Several related changes or components, moderate testing, or limited uncertainty |
| L | Cross-component change, substantial testing or coordination, or significant uncertainty |
| XL | Broad or high-uncertainty work that should be decomposed further before implementation |

If a task is XL, split it into smaller tasks whenever the request provides enough information to do so. Keep XL only when the uncertainty itself requires a bounded discovery or design task.

## Risk Assessment

Before finalizing the plan, identify tasks that:

- modify shared libraries, middleware, common APIs, authentication, authorization, global state, or other shared code paths
- add or alter database schemas, data migrations, backfills, indexes, constraints, or rollback behavior

For each identified task, add a row to the risk table with:

- the affected task title
- risk level: Low, Medium, or High
- the concrete failure or regression risk
- a mitigation or validation action
- a rollback or recovery approach

Database migration risks must address compatibility during deployment, existing-data handling, and rollback. Shared-code risks must address impacted consumers and regression coverage. If no tasks meet either condition, explicitly state that no mandatory risk items were identified.

## Output Format

Start with assumptions only when needed, then provide this table:

| Task title | Component | T-shirt size | Dependencies | Acceptance criteria |
|---|---|---|---|---|
| Concise action-oriented title | Owning area or system | XS, S, M, L, or XL | Earlier task titles or None | Semicolon-separated, observable completion conditions |

Follow it with the risk assessment:

| Task title | Risk level | Risk | Mitigation / validation | Rollback / recovery |
|---|---|---|---|---|
| A task requiring assessment | Low, Medium, or High | Specific failure mode | Concrete preventive or verification step | Concrete recovery path |

Keep task titles action-oriented, components specific, dependencies acyclic, and acceptance criteria testable. Do not include vague tasks such as "implement backend" or acceptance criteria such as "works correctly."