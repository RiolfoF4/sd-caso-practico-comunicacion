# AGENTS.md

## Overview

Distributed systems course project (UTN FRCU - ISI). Implements synchronous and asynchronous communication patterns between services using REST, RabbitMQ, and Apache Kafka. Built with .NET 10, SQLite, and Docker.

**Note:** Guide files under `docs/guias/` contain example code that may be outdated, use deprecated APIs, or have errors. Always follow the conventions in this file over the guides.

## Code Style

- **Namespaces:** Block-scoped (`namespace X { }`), never file-scoped
- **Braces:** Allman style (opening brace on its own line)
- **Null checks:** `is null` / `is not null`, never `== null`
- **Variables:** `var` when type is obvious; explicit type when it adds clarity
- **Async methods:** PascalCase with `Async` suffix (`CreateOrderAsync`)
- **Private fields:** `_camelCase` with underscore prefix
- **DTOs:** C# `record` positional types (immutable)
- **Domain models:** Mutable `class` with `{ get; set; }`
- **Interfaces:** `I` prefix, one per file (`IOrdersService`)
- **Access modifiers:** Explicit `public` on types, `private readonly` on fields
- **One type per file**
- **No XML doc comments** unless explicitly requested
- **Nullable reference types** enabled project-wide

## Commits

Use Conventional Commits.
