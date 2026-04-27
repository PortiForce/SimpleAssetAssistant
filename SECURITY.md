# Security Policy

## Supported Versions

PortiForce Simple Asset Assistant is currently an early-stage project and this public repository is provided mainly as a reference implementation.

At this stage, there are no officially supported production versions and no guaranteed security patch schedule.

| Version / Branch | Supported |
| ---------------- | --------- |
| `main` / public branch | Best-effort review only |
| Older commits / forks | Not supported |

If you use this code as a foundation for your own project, you are responsible for reviewing, hardening, testing, and securing it before using it in any production environment.

## Reporting a Vulnerability

If you discover a security vulnerability, please do **not** open a public GitHub issue with exploit details, secrets, tokens, connection strings, or sensitive information.

Instead, please report the issue privately using one of the following options:

- GitHub private vulnerability reporting, if enabled for this repository.
- A private message to the repository owner.

When reporting a vulnerability, please include as much detail as possible:

- A clear description of the issue.
- Steps to reproduce the problem.
- The affected area of the application.
- Potential impact.
- Suggested mitigation, if known.

I will review valid reports on a best-effort basis. Since this is an early-stage public reference project, I cannot guarantee fixed response times, formal SLAs, or security patches for all reported issues.

## Scope

The following areas are considered in scope:

- Authentication and authorization issues.
- Tenant isolation problems.
- Invite/onboarding flow vulnerabilities.
- Exposure of sensitive data.
- Incorrect access to another tenant's data.
- Unsafe API or UI behavior that could lead to privilege escalation or data leakage.

The following are out of scope:

- Vulnerabilities in unsupported forks.
- Issues caused by local misconfiguration.
- Missing production infrastructure hardening.
- Dependency vulnerabilities without a practical exploit path in this project.
- General best-practice suggestions without a concrete security impact.

## Responsible Disclosure

Please give reasonable time for review before publicly disclosing any vulnerability details.

This project is shared publicly to help other developers learn from its structure and ideas. Responsible reporting helps keep that learning environment safe and useful for everyone.
