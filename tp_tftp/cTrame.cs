using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace tp_tftp
{
    class cTrame
    {
        private byte[] lol = new byte[1];
        private byte[] op_err = new byte[2] { 0x00, 0x05 };
        private byte[] op_ack = new byte[2] { 0x00, 0x04 };
        private byte[] op_data = new byte[2] { 0x00, 0x03 };
        private byte[] op_wrq = new byte[2] { 0x00, 0x01 };
        private byte[] op_rrq = new byte[2] { 0x00, 0x02 };
        private string[] msgErreur = new string[8] { "","Fichier non trouve","Violation de l'acces","Violation de l'acces",
                                 "Disque plein","Operation TFTP illegale","Transfert ID inconnu","Utilisateur Inconnu" };


        public byte[] Erreur(UInt16 code)
        {
            byte[] a = op_err.Concat(BitConverter.GetBytes(code).Reverse()).Concat(Encoding.ASCII.GetBytes(msgErreur[code])).Concat(lol).ToArray();
            return a;
        }
        public byte[] Erreur(string msg)
        {
            byte[] a = op_err.Concat(BitConverter.GetBytes(0).Reverse()).Concat(Encoding.ASCII.GetBytes(msg)).Concat(lol).ToArray();
            return a;
        }

        public bool is_ACK(UInt16 bloc,byte[] trame)
        {
            byte[] a = trame.Take(4).ToArray();
            byte[] b = op_ack.Concat(BitConverter.GetBytes(bloc).Reverse()).ToArray();

            IStructuralEquatable se1 = a;
            if(se1.Equals (b, StructuralComparisons.StructuralEqualityComparer))
            {
                return true;
            }
            return false;
        }
        public bool is_DATA(UInt16 bloc, byte[] trame)
        {
            byte[] a = op_data.Concat(BitConverter.GetBytes(bloc).Reverse()).ToArray();
            IStructuralEquatable se1 = a;
            if (se1.Equals(trame.Take(4).ToArray(), StructuralComparisons.StructuralEqualityComparer))
            {
                return true;
            }
            return false;
 
        }
        public byte[] do_ACK(UInt16 bloc)
        {
            return op_ack.Concat(BitConverter.GetBytes(bloc).Reverse()).ToArray();
        }
        public byte[] do_DATA(UInt16 bloc, byte[] data)
        {
            byte[] a = op_data.Concat(BitConverter.GetBytes(bloc).Reverse()).Concat(data).ToArray();
            return a;
        }

    }
}
