using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileTransferProtocols.Interfaces;

namespace FileTransferProtocols.Protocols
{
    class Punter2
    {
        /*
        PUNTER PROTOCOL EXPLAINED
         * 
         * Sending a file.
         * First, you send a 
         * 
        */

        private IDataSender DataOut;


        private const int PUNTER_HDR_CHECKSUM_LO = 0;
        private const int PUNTER_HDR_CHECKSUM_HI = 1;
        private const int PUNTER_HDR_CRC_LO = 2;
        private const int PUNTER_HDR_CRC_HI = 3;


        private const byte DEFAULT_PACKET_SIZE = 0xfe;
        private const byte FIRST_PACKET_SIZE_SMALL = 0x07;
        private const byte FIRST_PACKET_SIZE_LARGE = 0x08;

        //0400
        private byte[] buffer1 = new byte[255];
        //0500
        private byte[] buffer2 = new byte[255];
        //bf00
        private byte[] buffer3 = new byte[255];

        private byte[] StatusCodes ={
        //        ;b930-b93e
        //        .byte $47, $4f, $4f	;GOO
        0x47,0x4f,0x4f,
        //        .byte $42, $41, $44	;BAD
        0x42,0x41,0x44,
        //        .byte $41, $43, $4b	;ACK
        0x41,0x43,0x4b,
        //        .byte $53, $2f, $42	;S/B
        0x53,0x2f,0x42,
        //        .byte $53, $59, $4e	;SYN
        0x53,0x59,0x4e
                                    };

//goocode		= 0
        private const int GOOCode = 0;
//badcode		= 3
        private const int BADCode = 3;
//ackcode		= 6
        private const int ACKCode = 6;
//sbcode		= 9
        private const int SBCode = 9;
//syncode		= 12
        private const int SYNCode = 12;

//gooenum		= 1
//badenum		= 2
//ackenum		= 3
//sbenum		= 4
//synenum		= 5

//shortdelay	= $3c
//longdelay	= $5a

//maxretries	= $14
        private byte MaxRetries = 0x14;
//prgmem		= $b900


//        .word prgmem
//        *= prgmem

//;b900 - receive
//        jmp receive2
//;b903 - send
//        jmp send2
//;b906 - initrecv
//        jmp initrecv2
//;b909 - initsend
//        jmp initsend2

        //$bff8 puntblks2
        private byte PunterBlockSize { get; set; }
        //$bff9
        private byte ProtocolFlag1 { get; set; }
        //$bffb
        private byte FileType { get; set; }

//        ;b90c
//        rts			;XXX UNUSED

        private byte BufferSize { get; set; } //bufsize

//        ;b90e
//retval		.byte 0			;return code; 0 = ok, 1 = abort
        private bool ReturnValue { get; set; }

//        ;b90f
//inbyte		.byte 0			;received byte
        private byte ReceivedByte { get; set; }

//        ;b910-b911
//timeout		.word 0			;timeout
        private int Timeout { get; set; }

//        ;b912
//code		.byte 0			;1 = goo, 2 = bad, 3 = ack
//                    ;4 = s/b, 5 = syn

        private byte Code { get; set; }

//        ;b913
//retrycntr	.byte 0			;retry counter
        private byte RetryCounter { get; set; }

//        ;b914
//retryflag	.byte 0			;retry flag;
//                    ;0 = ok
//                    ;1 = too many failures
        private bool RetryFlag { get; set; }

//        ;b915
//trstatus	.byte 0			;? trstatus;
//                    ;0 = ok
//                    ;1 = bad
//                    ;2 =
        private const byte TRANS_STATUS_OK = 0;
        private const byte TRANS_STATUS_BAD = 1;
        private const byte TRANS_STATUS_UNKNOWN = 2;

        private byte TransmitStatus { get; set; }
//        ;b916
//lastblkflag	.byte 0			;Have all blocks been received?
//                    ;0 = no, 1 = yes
        private bool LastBlockFlag { get; set; }
//        ;b917
//delaycntr	.byte 0			;???

        private byte DelayCounter { get; set; }

//        ;b918
//bufindex	.byte 0			;index for the receive buffer
        private byte RecvBufferIndex { get; set; }

//        ;b919
//initflag	.byte 0			;0 - send/receive
//                    ;1 - initrecv/initsend (prg)
//                    ;2 - initsend (seq)
        private const int INITFLAG_SENDRECV = 0;
        private const int INITFLAG_INITSENDRECV_PRG = 1;
        private const int INITFLAG_INITSEND_SEQ = 2;

        private int InitFlag { get; set; }

//        ;b91a-b91b
//blknr		.word 0			;block number
        private int BlockNumber { get; set; }

//        ;b91c
//pktsize		.byte $fe		;packet-size
        private int PacketSize { get; set; }

//        ;b91d-b91e
//goodblks	.word 0			;good blocks
        private int GoodBlocks { get; set; }

//        ;b91f-b920
//badblks		.word 0			;bad blocks
        private int BadBlocks { get; set; }

//        ;b921-b923
//stcode		.byte 0, 0, 0		;transfer status
        private byte[] STCode = { 0x00, 0x00, 0x00 };  

//        ;b924-b925
//chksum		.word 0			;checksum
        private int checksum { get; set; }

//        ;b926-b927
//crc		.word 0			;crc
        private int crc { get; set; }

//        ;b928-b92a
//recvdelay	.byte shortdelay	;receive delay
//        .byte shortdelay	;short, short, long
//        .byte longdelay

//        ;b92b-b92d
//senddelay	.byte longdelay		;send delay (transmit)
//        .byte longdelay		;long, long, short
//        .byte shortdelay

//        ;b92e
//        .byte 0			;XXX UNUSED

//        ;b92f
//tmpbufsize	.byte 0			;next bufsize


//        ;b93f
//stkptrtmp	.byte 0			;tmp storage for the stack-pointer




#region SocketInterface
        
        //public void OnDataReceived(byte[] data, int count)
        //{
        //    lock (SocketBuffer)
        //    {
        //        for (int i = 0; i < count; i++)
        //        {
        //            if (triggerflag)
        //            {
        //                Console.WriteLine("Ack");
        //            }
        //            SocketBuffer.Add(data[i]);
        //        }
        //    }
        //}

        //private void ClearReceiveBuffer()
        //{
        //    lock (SocketBuffer)
        //    {
        //        SocketBuffer.Clear();
        //    }
        //}

        ////private byte[] GetBlock(int count)
        ////{
        ////    byte[] b = new byte[count];
        ////    for (int i = 0; i < count; i++) b[i] = GetByte(500);
        ////    return b;
        ////}

        //private PunterTimedByte GetByte(int timeout_in_ms)
        //{
        //    PunterTimedByte pb = new PunterTimedByte();
        //    pb.Timeout = true;
        //    pb.Byte_Received = 0x00;
        //    try
        //    {
        //        DateTime Timeout = DateTime.Now;
        //        bool bytereceived = false;
        //        while ((Timeout.AddMilliseconds(timeout_in_ms) > DateTime.Now) && !bytereceived)
        //        {
        //            if (SocketBuffer.Count > 0)
        //            {
        //                lock (SocketBuffer)
        //                {
        //                    pb.Byte_Received = SocketBuffer[0];
        //                    pb.Timeout = false;
        //                    SocketBuffer.RemoveAt(0);
        //                    bytereceived = true;
        //                }
        //            }
        //            if (!bytereceived)Thread.Sleep(5);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        pb.Timeout = true;
        //    }
        //    return pb;

        //}

        //private byte GetByte()
        //{
        //    byte b = 0x00;
        //    try
        //    {
        //        DateTime Timeout = DateTime.Now;
        //        bool bytereceived = false;
        //        while ((Timeout.AddMilliseconds(500) > DateTime.Now) && !bytereceived)
        //        {
        //            if (SocketBuffer.Count > 0)
        //            {
        //                lock (SocketBuffer)
        //                {
        //                    b = SocketBuffer[0];
        //                    SocketBuffer.RemoveAt(0);
        //                    bytereceived = true;
        //                }
        //            }
        //            Thread.Sleep(10);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        b = 0x00;
        //    }
        //    return b;
        //}

        private void writeBuffer(byte[] xbuff, int size)
        {
            DataOut.Send(xbuff, size);
        }

        private void writeByte(byte b)
        {
            DataOut.Send(b);
        }
#endregion
//receive2
//;b940
//        lda #0
//        jmp tmpb94a		;XXX
//initrecv2
//;b945
//        lda #1
//        jmp tmpb94a		;XXX
//tmpb94a					;XXX
//;b94a
//        sta initflag
//        jsr setup1
//        tsx
//        stx stkptrtmp
//        jmp recvblk

        private void Send2()
        {
            InitFlag = 0;
            Setup1();

            //        tsx
            //        stx stkptrtmp

            BlockNumber = 1;


            //Blocknumber in outbound packet
            buffer2[5] = 0; //high
            buffer2[6] = 0; //low

            ReadBlock();
            Send();
        }

        private void InitSend2() //For last block?
        {
            InitFlag = FileType;
            buffer2[7] = FileType;
            buffer2[4] = 0x00;
            buffer2[5] = 0xff;
            buffer2[6] = 0xff;
            Setup1();
            //        tsx
            //        stx stkptrtmp
            Send();
        }

        private void Setup1()
        {
            BlockNumber = 0;

            TransmitStatus = TRANS_STATUS_OK;
            //TODO: maybe:        sta trstatus		;b915

            LastBlockFlag = false;
            RetryFlag = false;
            DelayCounter = 0;
            GoodBlocks = 0;
            BadBlocks = 0;
            ProtocolFlag1 = 0;
            
            PacketSize = DEFAULT_PACKET_SIZE;
            BufferSize = ((InitFlag == INITFLAG_SENDRECV)?FIRST_PACKET_SIZE_SMALL : FIRST_PACKET_SIZE_LARGE);
        }

        private byte RecvByte()
        {
            //get rid of this
            return 0;
            //recvbyte
            //;b9cb
            //        lda #0
            //        sta time + 2
            //        sta time + 1
            //        ldx #5			;modem
            //        jsr chkin
            //recvbytel1
            //;b9d6
            //.(
            //        lda shflag
            //        beq skip
            //        ldx stkptrtmp
            //        txs
            //        jmp abort
            //skip
            //.)
            //;b9e2
            //        lda timeout + 1
            //        cmp time + 1
            //        bne notimeoutb1
            //        lda time + 2
            //        cmp timeout
            //        bcc notimeoutb1
            //        lda #1
            //        jmp timeoutb1
            //notimeoutb1
            //;b9f5
            //        lda ridbe
            //        cmp ridbs
            //        beq recvbytel1		;b9d6
            //        jsr getin
            //        sta inbyte
            //        lda #0
            //timeoutb1
            //;ba05
            //        sta retval
            //        rts
        }

        private void RecvSTCode()
        {
            //recvstcode
            //;ba09
            //        sta timeout + 1
            //        stx timeout
            //        lda #0
            //        sta stcode + 2
            //        sta stcode + 1
            //        sta stcode
            //nocodebr2
            //        jsr recvbyte
            //        lda retval
            //        bne timedout		;ba6f
            //        jsr rotstcode
            //        lda #<stcodes
            //        sta codeoffset
            //        lda #>stcodes
            //        sta codeoffset + 1
            //        lda #1
            //        sta code		;b912
            //codechkl0
            //        ldy #0
            //codechkl1
            //;ba36
            //codeoffset = * + 1
            //;ba37
            //        lda $0000,y
            //        cmp stcode,y		;b921
            //        bne nocodebr1		;ba46
            //        cpy #2
            //        beq aret4		;ba74 rts
            //        iny
            //        jmp codechkl1		;ba36
            //nocodebr1
            //;ba46
            //        inc code
            //        lda code
            //        cmp #6
            //        beq nocodebr2		;ba1a
            //        ldx codeoffset
            //        inx
            //        inx
            //        inx
            //        stx codeoffset
            //        jmp codechkl0
        }

        private void RotSTCode()
        {
            byte b = STCode[0];
            STCode[0] = STCode[1];
            STCode[1] = STCode[2];
            STCode[2] = b;
        }

        private void TimedOut()
        {
            //timedout
            //;ba6f
            //        lda #0
            //        sta code
            //aret4
            //;ba74
            //        rts
        }

        private void SendCode(int codeoffset)
        {
            for (int i = 0; i < 3; i++)
            {
                DataOut.Send(StatusCodes[codeoffset + i]);
            }
        }

        private void SyncSend()
        {
            //syncsend
            //;ba8e
            //        ldy delaycntr
            //        iny
            //        cpy #3
            //        bne syncsendbr1
            //        ldy #0
            //syncsendbr1
            //        lda senddelay,y
            //        tax
            //        lda #0
            //        sty delaycntr
            //        rts
        }

        private void SyncRecv()
        {
            //syncrecv
            //;baa2
            //        ldy delaycntr
            //        iny
            //        cpy #3
            //        bne syncrecvbr1
            //        ldy #0
            //syncrecvbr1
            //        lda recvdelay,y
            //        tax
            //        lda #0
            //        sty delaycntr
            //        rts
        }

        private void SetTimeout()
        {
            //settimeout
            //;bab6
            //        lda #1
            //        ldx #0
            //        sta timeout + 1
            //        ;stx timeout
            //        sta timeout		;XXX This *has* to be incorrect!
            //        rts
        }
        private void ResetRetries()
        {
            RetryCounter = 0;
        }

        private void Retry()
        {
            RetryFlag = false;
            RetryCounter++;
            if (RetryCounter != MaxRetries) return;
            RetryFlag = true;
        }
        private void ReceiveBlock()
        {
            //recvblk
            //;badc
            //        jsr resetretries	;bac1
            //recvblkretry
            //.(
            //        lda trstatus		;b915
            if (TransmitStatus == 0)
            {
                SendCode(GOOCode);
            }
            else
            {
                SendCode(BADCode);
            }
            //        jsr syncrecv		;baa2
            //        jsr recvstcode		;ba09
            RecvSTCode();
            //if (Code == 
            //        lda code		;b912
            //        cmp #ackenum		;ACK
            //        beq goodblock		;bb06

            //        jsr retry
            Retry();
            if (RetryFlag){
            //        lda retryflag		;b914
            //        beq goodskip		;bae9
            //        jmp abort		;bcc1
            //goodblock
            //;bb06
            //        lda lastblkflag		;b916
            //        beq notlastblock
            //lastblock
            //        jsr resetretries	;bac1
            //;bb0e
            //loop
            //        lda #<(stcodes + sbcode)
            //        jsr sendcode		;ba75
            SendCode(SBCode);

            //        jsr syncrecv		;baa2
            //        jsr recvstcode		;ba09
            //        lda code		;b912

            //        cmp #synenum		;SYN
            //        beq gotsyn		;bb2b
            //        jsr retry		;bac7
            //        lda retryflag		;b914
            //        beq loop		;bb0e
            //        jmp success		;bcca
            //;bb2b
            //gotsyn
            //        lda #<(stcodes + syncode)
            //        jsr sendcode		;ba75
            SendCode(SYNCode);

            
            FileType = buffer2[7];
            //        lda #1
            //        ldx #0
            //        jsr recvstcode		;ba09
            //        jmp success		;bcca
            //notlastblock
            //;bb40
            //        jsr resetretries	;bac1
            //loop2
            //        lda #<(stcodes + sbcode)
            //        jsr sendcode		;ba75
            SendCode(SBCode);

            //        jsr settimeout		;bab6
            //        ldx #0
            //        stx bufindex		;b918
            //loop3
            //        jsr recvbyte		;b9cb
            //        lda retval		;b90e
            //        beq $bb70
            //        lda bufindex		;b918
            //        beq failure
            //        lda #1
            //        sta trstatus		;b915
            //        jmp badblock2		;bbb5
            //;bb65
            //failure
            //        jsr retry		;bac7
            //        lda retryflag		;b914
            //        beq loop2		;bb43
            //        jmp abort		;bcc1
            //;bb70
            //        ldx bufindex		;b918
            //        lda inbyte		;b90f
            //        sta $0500,x
            //        cpx #2
            //        bne $bb9f
            //        lda #0
            //        sta timeout + 1
            //        lda #shortdelay
            //        sta timeout
            //        lda $0500
            //        cmp #$41		;"a"
            //        bne recvdata		;bb9f
            //        lda $0501
            //        cmp #$43		;"c"
            //        bne recvdata		;bb9f
            //        lda $0502
            //        cmp #$4b		;"k"
            //        bne recvdata		;bb9f
            //        jmp notlastblock	;bb40
            //recvdata
            //;bb9f
            //        inc bufindex		;b918
            //        lda bufindex		;b918
            //        cmp bufsize		;b90d
            //        bne loop3		;bb50
            //;bbaa
            //        jsr checksum		;bbe4
            //        jsr chkblk		;bc1d
            //        lda trstatus		;b915
            //        beq goodskip2
            //;bbb5
            //badblock2
            //        jsr clrchn		;ffcc
            //        jsr incbadblks		;bdce
            //        jsr printstats		;bdd7
            //        jmp recvblk		;badc
            //goodskip2
            //;bbc1
            //        lda initflag		;b919
            //        bne isrecvinit		;bbcf
            //        jsr writeblk		;bc61
            //        jsr incgoodblks		;bdc5
            //        jsr printstats		;bdd7
            //isrecvinit
            //;bbcf
            //        lda $0504
            //        sta bufsize		;b90d
            //        lda $0506		;block number high
            //        cmp #$ff		;last block
            //        bne recv		;bbe1
            //        lda #1
            //        sta lastblkflag		;b916
            //;bbe1
            //recv
            //        jmp recvblk		;badc
            //.)
            }
        }

        private void Checksum()
        {
            //checksum
            //;bbe4
            //        lda #0
            //        sta chksum
            //        sta chksum + 1
            //        sta crc
            //        sta crc + 1
            //        ldy #4
            //checksuml1
            //        lda chksum
            //        clc
            //        adc $0500,y
            //        sta chksum
            //        bcc checksumbr1
            //        inc chksum + 1
            //checksumbr1
            //        lda crc
            //        eor $0500,y
            //        sta crc
            //        lda crc + 1
            //        rol
            //        rol crc
            //        rol crc + 1
            //        iny
            //        cpy bufsize
            //        bne checksuml1
            //        rts
        }

        private void CheckBlock()
        {
            //chkblk
            //;bc1d
            //.(
            //        lda chksum
            //        cmp $0500		;checksum low
            //        bne blkbad
            //        lda chksum + 1
            //        cmp $0501		;checksum high
            //        bne blkbad
            //        lda crc
            //        cmp $0502		;crc low
            //        bne blkbad
            //        lda crc + 1
            //        cmp $0503		;crc high
            //        bne blkbad
            //        lda #0

            //        jmp skip
            //blkbad
            //        lda #1
            //skip
            //        sta trstatus
            //.)
            //        rts
        }

        private void InitCheckHeader()
        {
            buffer2[PUNTER_HDR_CHECKSUM_LO] = (byte)(checksum >> 8);
            buffer2[PUNTER_HDR_CHECKSUM_HI] = (byte)(checksum & 0xff);
            buffer2[PUNTER_HDR_CRC_LO] = (byte)(crc >> 8);
            buffer2[PUNTER_HDR_CRC_HI] = (byte)(crc & 0xff);
        }

        private void WriteBlock()
        {
            //writeblk
            //;bc61
            //        ldy #7
            //        cpy bufsize		;b90d
            //        beq aret2
            //        ldx #2			;file
            //        jsr chkout		;ffc9
            //writeblkl1
            //        lda $0500,y
            //        jsr chrout		;ffd2
            //        iny
            //        cpy bufsize		;b90d
            //        bne writeblkl1
            //        jmp clrchn		;ffcc
            //aret2		rts
        }

        private void ReadBlock()
        {
            //readblk
            //;bc7d
            //        ldy #7
            //        ldx #2			;file
            //        jsr chkin
            //readblkl1
            //;bc84
            //        jsr chrin
            
            //        sta $0400,y
            //        lda status
            //        bne readblkexit2
            //        iny
            //        cpy pktsize		;b91c
            //        bne readblkl1
            //        sty tmpbufsize		;b92f

                //        inc blknr		;b91a
                //        bne readblkexit1
                //        inc blknr + 1		;b91b
                //readblkexit1
            BlockNumber++;
                //THis block is used to repor the block number on screen?
                //        lda blknr		;b91a
                //        sta $0405		;low byte
                //        lda blknr + 1		;b91b
                //        sta $0406		;high byte


            //readblkexit
            //        sty $0504
            //        jmp clrchn		;ffcc
            //        rts			;XXX UNUSED
            //readblkexit2
            //;bcb2
            //        iny			;last block!
            //        sty tmpbufsize
            //        lda #$ff		;last block marker
            //        sta $0406
            //        sta $0405
            //        jmp readblkexit
        }

        public void Abort()
        {
            //abort
            //;bcc1
            //        jsr clrchn
            //        lda #1
            //        sta buf
            //        rts
           
        }

        public void Success()
        {
            //success
            //;bcca
            //        lda #0
            //        sta buf
            //        rts
        }

        private void Send()
        {
            //send
            //;bcd0
            //        jsr resetretries	;bac1
            //sendl1
            //        jsr syncsend		;ba8e
            //        jsr recvstcode		;ba09
            //        lda code		;b912
            //        cmp #gooenum		;goo
            //        beq recvgoobr1
            //        jsr retry		;bac7
            //        lda retryflag
            //        beq sendl1		;bcd3
            //        jmp abort		;bcc1
            //recvgoobr1
            //;bceb
            //        jsr resetretries	;bac1
            //sendack
            //;bcee
            //        lda #<(stcodes + ackcode)
            //        jsr sendcode		;ba75
            SendCode(ACKCode);

            //        jsr syncsend		;ba8e
            //        jsr recvstcode		;ba09
            //        lda code		;b912
            //        cmp #sbenum		;s/b
            //        beq gotsb		;bd0b
            //        jsr retry		;bac7
            //        lda retryflag
            //        beq sendack		;bcee
            //        jmp abort		;bcc1
            //gotsb
            //;bd0b
            //        lda lastblkflag		;b916
            //        beq notlastsend		;bd3e
            //        jsr resetretries	;bac1
            //sendsyn
            //        lda #<(stcodes + syncode)
            //        jsr sendcode		;ba75
            SendCode(SYNCode);
            //        jsr syncsend		;ba8e
            //        jsr recvstcode		;ba09
            //        lda code		;b912
            //        cmp #synenum		;syn
            //        beq gotlastcode		;bd30

            //        jsr retry		;bac7
            //        lda retryflag
            //        beq sendsyn		;bd13
            //        jmp abort		;bcc1
            //gotlastcode
            //;bd30
            //        lda #<(stcodes + sbcode)
            //        jsr sendcode		;ba75
            SendCode(SBCode);

            //        jsr settimeout		;bab6
            //        jsr recvstcode		;ba09
            //        jmp success		;bcca
            //notlastsend
            //;bd3e
            //        jsr resetretries	;bac1
            //        jsr sendblk		;bd94
            //retrysend
            //;bd44
            //        jsr settimeout
            //        jsr recvstcode
            //        lda code		;b912
            //        sta trstatus		;b915
            //        cmp #gooenum		;goo
            //        beq goodsend		;bd6c
            //        cmp #badenum		;bad
            //        bne senderror		;bd61
            //badsend
            //;bd58
            //        jsr incbadblks		;bdce
            //        jsr printstats		;bdd7
            //        jmp recvgoobr1		;bceb
            //senderror
            //;bd61
            //        jsr retry		;bac7
            //        lda retryflag		;b914
            //        beq retrysend		;bd44
            //        jmp abort		;bcc1
            //goodsend
            //;bd6c
            //        sta trstatus		;b915
            //        jsr incgoodblks		;bdc5
            //        jsr printstats		;bdd7
            //        lda $0506		;block number high
            //        cmp #$ff		;last block
            //        bne notlastblkbr1	;bd84
            //        lda #1
            //        sta lastblkflag		;b916
            //        jmp recvgoobr1		;bceb
            //notlastblkbr1
            //;bd84
            //        jsr movebuf		;bdb1
            //        lda $0506		;block number high
            //        cmp #$ff		;last block
            //        beq noreadblk
            //        jsr readblk		;bc7d
            //noreadblk
            //;bd91
            //        jmp recvgoobr1		;bceb
        }

        private void SendBlock()
        {
            Checksum();
            InitCheckHeader();
            for (byte i = 0; i < BufferSize; i++)
            {
                DataOut.Send(buffer2[i]);

            }
        }

//movebuf
//;bdb1
//        ldy #0
//movebufl1
//        lda $0400,y
//        sta $0500,y
//        iny
//        bne movebufl1
//        lda tmpbufsize
//        sta bufsize
//        rts
//        rts			;XXX UNUSED
//        rts			;XXX UNUSED

        private void IncrementGoodBlocks()
        {
            GoodBlocks++;
        }

        private void IncrementBadBlocks()
        {
            BadBlocks++;
        }

//printstats
//;bdd7
//        ldy #6
//        jsr plot15		;bdf8
//        ldx goodblks		;b91d
//        lda goodblks + 1	;b91e
//        jsr blinprt		;c770
//        ldy #$14
//        jsr plot15		;bdf8
//        ldx badblks		;b91f
//        ;lda badblks + 1	;b920
//        lda protflg1		;bff9
//        jsr blinprt		;c770
//        ldx #$16
//        ldy #0
//        .byte $2c		;bit $xxxx
//plot15
//        ldx #$15
//        clc
//        jmp plot

//;-----

    }
}
