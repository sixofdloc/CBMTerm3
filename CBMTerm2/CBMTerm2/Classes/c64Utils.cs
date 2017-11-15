using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBMTerm2.Classes
{
    class c64Utils
    {
//        procedure TForm1.FormKeyDown(Sender: TObject; var Key: Word;
//  Shift: TShiftState);
//begin
//  Case Key of
//    VK_PAUSE: BEGIN
//                IF Speed > 0 THEN BEGIN
//                  IF BufPause THEN BufPause := FALSE ELSE BufPause := TRUE;
//                END;
//              END;
//    VK_HOME: IF Shift=[ssShift] THEN FKPut(#147) ELSE FKPut(#19);
//    VK_UP: FKPut(#145);
//    VK_DOWN: FKPut(#17);
//    VK_LEFT: FKPut(#157);
//    VK_RIGHT: FKPut(#29);
//    49: BEGIN {1}
//          IF Shift=[ssCtrl] THEN FKPut(#144)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#129);
//        END;
//    50: BEGIN {2}
//          IF Shift=[ssCtrl] THEN FKPut(#5)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#149);
//        END;
//    51: BEGIN {3}
//          IF Shift=[ssCtrl] THEN FKPut(#28)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#150);
//        END;
//    52: BEGIN {4}
//          IF Shift=[ssCtrl] THEN FKPut(#159)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#151);
//        END;
//    53: BEGIN {5}
//          IF Shift=[ssCtrl] THEN FKPut(#156)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#152);
//        END;
//    54: BEGIN {6}
//          IF Shift=[ssCtrl] THEN FKPut(#30)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#153);
//        END;
//    55: BEGIN {7}
//          IF Shift=[ssCtrl] THEN FKPut(#31)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#154);
//        END;
//    56: BEGIN {8}
//          IF Shift=[ssCtrl] THEN FKPut(#158)
//          ELSE IF Shift=[ssAlt] THEN FKPut(#155);
//        END;
//    57: BEGIN {9}
//          IF Shift=[ssCtrl] THEN FKPut(#18);
//        END;
//    48: BEGIN {0}
//          IF Shift=[ssCtrl] THEN FKPut(#146);
//        END;
//    65: BEGIN {A}
//          IF Shift=[ssAlt] THEN FKPut(#176);
//        END;
//    66: BEGIN {B}
//          IF Shift=[ssAlt] THEN FKPut(#191);
//        END;
//    67: BEGIN {C}
//          IF Shift=[ssAlt] THEN FKPut(#188);
//        END;
//    68: BEGIN {D}
//          IF Shift=[ssAlt] THEN FKPut(#172);
//        END;
//    69: BEGIN {E}
//          IF Shift=[ssAlt] THEN FKPut(#177);
//        END;
//    70: BEGIN {F}
//          IF Shift=[ssAlt] THEN FKPut(#187);
//        END;
//    71: BEGIN {G}
//          IF Shift=[ssAlt] THEN FKPut(#165);
//        END;
//    72: BEGIN {H}
//          IF Shift=[ssAlt] THEN FKPut(#180);
//        END;
//    73: BEGIN {I}
//          IF Shift=[ssAlt] THEN FKPut(#162);
//        END;
//    74: BEGIN {J}
//          IF Shift=[ssAlt] THEN FKPut(#181);
//        END;
//    75: BEGIN {K}
//          IF Shift=[ssAlt] THEN FKPut(#161);
//        END;
//    76: BEGIN {L}
//          IF Shift=[ssAlt] THEN FKPut(#182);
//        END;
//    77: BEGIN {M}
//          IF Shift=[ssAlt] THEN FKPut(#167);
//        END;
//    78: BEGIN {N}
//          IF Shift=[ssAlt] THEN FKPut(#170);
//        END;
//    79: BEGIN {O}
//          IF Shift=[ssAlt] THEN FKPut(#185);
//        END;
//    80: BEGIN {P}
//          IF Shift=[ssAlt] THEN FKPut(#175);
//        END;
//    81: BEGIN {Q}
//          IF Shift=[ssAlt] THEN FKPut(#171);
//        END;
//    82: BEGIN {R}
//          IF Shift=[ssAlt] THEN FKPut(#178);
//        END;
//    83: BEGIN {S}
//          IF Shift=[ssAlt] THEN FKPut(#174);
//        END;
//    84: BEGIN {T}
//          IF Shift=[ssAlt] THEN FKPut(#163);
//        END;
//    85: BEGIN {U}
//          IF Shift=[ssAlt] THEN FKPut(#184);
//        END;
//    86: BEGIN {V}
//          IF Shift=[ssAlt] THEN FKPut(#190);
//        END;
//    87: BEGIN {W}
//          IF Shift=[ssAlt] THEN FKPut(#179);
//        END;
//    88: BEGIN {X}
//          IF Shift=[ssAlt] THEN FKPut(#189);
//        END;
//    89: BEGIN {Y}
//          IF Shift=[ssAlt] THEN FKPut(#183);
//        END;
//    90: BEGIN {Z}
//          IF Shift=[ssAlt] THEN FKPut(#173);
//        END;
//  END;
//end;


        static string UCL = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string LCL = "abcdefghijklmnopqrstuvwxyz";
        public static char ASC2PET(char c)
        {
            char t = ' ';
            if (UCL.Contains(c.ToString()))
            {
                t = c.ToString().ToLower()[0];
            }
            else
            {
                if (LCL.Contains(c.ToString()))
                {
                    t = c.ToString().ToUpper()[0];
                }
                else
                {
                    switch (c)
                    {
                        case (char)0x08:
                            t = (char)0x14;
                            break;
                        default:
                            t = c;
                            break;
                    }
                }
            }
            return t;
        }
        //        function TForm1.ASC2PET(s:string):string;
        //var i: integer;
        //    t: string;
        //begin
        //t := s;
        //FOR i := 1 to Length(s) DO BEGIN
        //    IF Pos(t[i],UCL) > 0 THEN t[i] := LCL[Pos(t[i],UCL)] ELSE
        //    IF Pos(t[i],LCL) > 0 THEN t[i] := UCL[Pos(t[i],LCL)] ELSE
        //    IF t[i] = #8 THEN t[i] := #20;// ELSE
        ////    IF t[i] =  THEN t[i] := #17;
        //END;
        //ASC2PET := t;
        //end;

//procedure TForm1.FKPut(c: char);
//begin
//IF IdTelnet1.Connected THEN BEGIN
//  IdTelnet1.Write(c);
//END;
//IF LocalEcho1.Checked THEN BEGIN
//  Chrout(c);
//END;

//end;
    
    }
}
