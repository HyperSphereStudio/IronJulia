import subprocess

print("Building Lexer")

subprocess.run(["java", "-jar", "antlr.jar",
                 "-o", "gen",
                 "-package", "IronJulia.Parse",
                 "-encoding", "utf8", "JuliaLexer.g4"], cwd="Exprs/Frontend")

print("Building Parser")

subprocess.run(["java", "-jar", "antlr.jar",
                "-o", "gen",
                "-package", "IronJulia.Parse",
                "-encoding", "utf8", "-visitor", "JuliaParser.g4"], cwd="Exprs/Frontend")