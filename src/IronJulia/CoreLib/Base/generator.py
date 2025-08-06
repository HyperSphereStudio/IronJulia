import os
from .Numbers import numbers_list, signed_int_list, unsigned_int_list, floating_point_list, NumericType

dir = os.path.dirname(os.path.abspath(__file__))
file = open(os.path.join(dir, "Numbers.cs"), "w", encoding="utf-8")

file.write("public partial struct Base {")

def emit_number(jname, num: NumericType):
   conversions = ""
   for num2 in numbers_list:
      if num == num2:
          continue
      conversions += f"""
        public static {num2.get_promote_to(num)} operator {jname}({num2.name} value) => new(({num.name}) value);
        public static {num.get_promote_to(num2)} operator {num2.name}({jname} value) => ({num2.name}) value.Value;
"""

   extra_conv = ""
   if num.type == "Unsigned":
       signed_name = jname[1:]
       extra_conv += f"""
        public static {signed_name} operator -({jname} a) => -({signed_name})({num.name})a.Value;
"""
   else:
       extra_conv += f"""
        public static {jname} operator -({jname} a) => ({num.name}) (-({num.name})a.Value);
"""
   
   file.write(f"""
    public readonly struct {jname}({num.name} value) : {num.type}, IEquatable<{jname}> {{
        public readonly {num.name} Value = value;
        
        public static implicit operator {jname}({num.name} value) => new(value);
        public static implicit operator {num.name}({jname} value) => value.Value;
        
        public static {jname} operator +({jname} a, {jname} b) => ({num.name}) (a.Value + b.Value);
        public static {jname} operator ++({jname} a) => ({num.name}) (a.Value + ({num.name})1);
        public static {jname} operator --({jname} a) => ({num.name}) (a.Value - ({num.name})1);

{extra_conv}

        public static {jname} operator +({jname} a) => ({num.name}) (+({num.name})a.Value);
        public static {jname} operator -({jname} a, {jname} b) => ({num.name}) (a.Value - b.Value);
        public static {jname} operator *({jname} a, {jname} b) => ({num.name}) (a.Value * b.Value);
        public static {jname} operator /({jname} a, {jname} b) => ({num.name}) (a.Value / b.Value);
        public static bool operator <({jname} a, {jname} b) => a.Value < b.Value;
        public static bool operator >({jname} a, {jname} b) => a.Value > b.Value;
        public static bool operator ==({jname} a, {jname} b) => a.Value == b.Value;
        public static bool operator !=({jname} a, {jname} b) => !(a == b);
        
        {conversions}
        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals({jname} other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is {jname} other && Equals(other);
    }}

""")

for i, ty in enumerate(signed_int_list):
    jname = f"Int{ty.bitcount*8}" if ty.name != "nint" else "Int"
    emit_number(jname, ty)

for i, ty in enumerate(unsigned_int_list):
    jname = f"UInt{ty.bitcount*8}" if ty.name != "nuint" else "UInt"
    emit_number(jname, ty)

for i, ty in enumerate(floating_point_list):
    jname = f"Float{ty.bitcount*8}"
    emit_number(jname, ty)

file.write("}")
file.close()
