## v1.1.45 (patch)

Changes since v1.1.44:

- Hold OnClose open with a condition instead of Thread.Sleep in the concurrency test ([@Claude](https://github.com/Claude))
- Run OnClose once even when Dispose arrives while it is running [patch] ([@Claude](https://github.com/Claude))

