import os
import sys

# Set cd to current directory
os.chdir(os.path.dirname(os.path.abspath(__file__)))

print("Current working directory:", os.getcwd())

import CoreLib.Base.generator
import Exprs.Frontend.grammer_builder


