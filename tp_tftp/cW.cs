using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace tp_tftp
{
    class cW
    {
        private bool _bThread;
        private EndPoint _pd;
        private string _fichier;

        public cW(sInfoTh info)
        {
            _bThread = false;
            _pd = info.pDistant;
            _fichier = info.Nom;
            Thread t = new Thread(new ThreadStart(thW));
            t.Start();
        }


        private void thW()
        {
            FileStream fs;
            byte[] databloc = new byte[516];
            int bLue = 0;
            cTrame trame = new cTrame();
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.ReceiveTimeout = 1;

            EndPoint pl = new IPEndPoint(0, 0);
            s.Bind(pl);

            UInt16 bloc = 0;
            UInt16 blocSuivant = 1;
            int retry = 0;
            if (_fichier != null)
            {
                if (File.Exists(_fichier))
                {
                    fs = null;
                    s.SendTo(trame.Erreur("Fichier existe deja"), SocketFlags.None, _pd);
                    _bThread = false;
                }
                else
                {
                    fs = new FileStream(_fichier, FileMode.Create, FileAccess.Write);
                    _bThread = true;
                  
                }
            }
            else
            {
                fs = null;
                s.SendTo(trame.Erreur("Nom de fichier vide"),SocketFlags.None, _pd);
                _bThread = false;
            }
            
            while (_bThread)
            {
                s.SendTo(trame.do_ACK(bloc), 4, SocketFlags.None, _pd);

                s.Poll(3000000,SelectMode.SelectRead);
                if (s.Available > 0)
                {
                    bLue = s.Receive(databloc, SocketFlags.None);
                }

                if(trame.is_DATA((blocSuivant),databloc))
                {
                    byte[] a = databloc.Skip(4).ToArray();
                    fs.Write(a,0,a.Length);
                    retry = 0;
                    bloc++;
                    blocSuivant++;
                    if (bLue < 512)
                    {
                        _bThread = false;
                        s.SendTo(trame.do_ACK(bloc), 4, SocketFlags.None, _pd);
                        fs.Close();
                    }
                }
                else
                {
                    retry ++;
                }

                if (retry == 3)
                {
                    _bThread = false;
                    fs.Close();
                    File.Delete(_fichier);
                    s.SendTo(trame.Erreur("Erreur de transmission"), _pd);
                }

            }
            s.Close();
        }
    }
}
