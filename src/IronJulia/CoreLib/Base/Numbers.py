
class NumericType:
    def __init__(self, name, bitcount, min_value, max_value, type):
        self.name = name
        self.bitcount = bitcount
        self.min_value = min_value
        self.max_value = max_value
        self.type = type

    def get_promote_to(self, dest):
        if self.min_value >= dest.min_value and self.max_value <= dest.max_value:
            return "implicit"
        else:
            return "explicit"
        

floating_point_list = [NumericType("Half", 2, -65504, 65504, "AbstractFloat"),
                       NumericType("float", 4, -3.4E38, 3.4E38, "AbstractFloat"),
                       NumericType("double", 8, -1.7E308, 1.7E308, "AbstractFloat"),
                       NumericType("decimal", 16, -7.9228E28, 7.9228E28, "AbstractFloat")]

signed_int_list =     [NumericType("sbyte", 1, -(1 << 7), 1 << 7 - 1, "Signed"),
                       NumericType("short", 2, -(1 << 15), 1 << 15 - 1, "Signed"),
                       NumericType("int", 4, -(1 << 31), 1 << 31 - 1, "Signed"),
                       NumericType("long", 8, -(1 << 63), 1 << 63 - 1, "Signed"),
                       NumericType("nint", 8, -(1 << 63), 1 << 63 - 1, "Signed"),
                       NumericType("System.Int128", 16, -(1 << 127), 1 << 127 - 1, "Signed")]

unsigned_int_list =   [NumericType("byte", 1, 0, 1 << 8 - 1, "Unsigned"),
                       NumericType("ushort", 2, 0, 1 << 16 - 1, "Unsigned"),
                       NumericType("uint", 4, 0, 1 << 32 - 1, "Unsigned"),
                       NumericType("ulong", 8, 0, 1 << 64 - 1, "Unsigned"),
                       NumericType("nuint", 8, 0, 1 << 64 - 1, "Unsigned"),
                       NumericType("System.UInt128", 16, 0, 1 << 128 - 1, "Unsigned")]

numbers_list = [*floating_point_list, *signed_int_list, *unsigned_int_list]
