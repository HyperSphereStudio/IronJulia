import subprocess
import os

os.chdir("IronJulia/Parse")

subprocess.run(["java", "-jar", "antlr.jar",
                 "-o", "gen",
                 "-package", "IronJulia.Parse",
                 "-encoding", "utf8", "JuliaLexer.g4"])

subprocess.run(["java", "-jar", "antlr.jar",
                "-o", "gen",
                "-package", "IronJulia.Parse",
                "-encoding", "utf8", "-visitor", "JuliaParser.g4"])