include "mapbasic.def"
include "menu.def"
include "Icons.def"
Declare Sub Main
Declare Sub button_sub1
Declare Sub button_exchange
Declare Function exchangeNode(currObj as Object,currTab as string,rowIndex as Integer) as integer
Declare Function exchangeNode1(currObj as Object) as integer
Type Region							'���� type��������
	name as String
	id as String
	nodeCount as Integer			'region�ļ�����
	lng(255) as String				'��������
	lat(255) as String				'γ������
	list as String			 	'��γ���б�
	regObject as object		'�洢Region �Ķ�������
End Type
dim max_rows as Integer
Dim regions() as Region  '����type ���͵����� rigions() '����±�Ϊ����������
Dim listCount as Integer
dim tmpList() as String
dim i,j,k as Integer
dim firstStr as String
dim lastStr as String
Dim tmpX,tmpY as String
dim nameCol as Alias
dim idCol as Alias
dim listCol as Alias
dim nodeCountCol as Alias
dim firstTable as Alias




Sub Main
	Create ButtonPad "CreateRegions"  As

	PushButton
  	Icon 104
      	Calling button_sub1
		HelpMsg "��γ������Regions"

	PushButton
  	Icon 94
      	Calling button_exchange
		HelpMsg "������ѡRegion����������λ��"

	Title "CreateRegions" 
	ToolbarPosition(0,1)
	Width 2
	Show
End Sub


Sub button_sub1

	onerror goto error_trap


firstTable = TableInfo(0, TAB_INFO_NAME)			'��õ�ǰ�򿪱������
max_rows = TableInfo(firstTable, TAB_INFO_NROWS)       'ȡ����������
print(max_rows)
nameCol = firstTable + "." + "name"	
idCol = firstTable + "." + "id"	
nodeCountCol = firstTable + "." + "nodeCount"	
listCol = firstTable + "." + "list"	





Redim regions(max_rows)								'ȷ�����鳤��

for i=1 to max_rows Step 1									   '�ӱ�ĵ�һ�п�ʼ������
    print 
	fetch rec i from firstTable								'�ӱ�ĵ�һ�п�ʼ������
	regions(i).name = nameCol							'������һ�е�name�ֶθ��Ƹ� regions(i).name 
	regions(i).id = idCol								'������һ�е�name�ֶθ��Ƹ� regions(i).name
	regions(i).nodeCount = nodeCountCol				'�ѵ�һ�е�nodeCount��ֵ����regions(i).nodeCount

	lastStr = listCol
	'note "list:" & listCol
	'print "list:" & listCol
	j = 0
  	do 
		j = j + 1
		firstStr = left$(lastStr,instr(1,lastStr,"|")-1)			   'ǰ�벿�ֱ�����firstStr 
		lastStr= right$(lastStr,len(lastStr) - instr(1,lastStr,"|"))   '�õ�һ��"|" �ָγ���ַ���,��벿�ֱ�����lastStr

		'print "firstStr:____" & firstStr 


		if firstStr <> "" then														'���firstStr Ϊ����˵���Ѿ������һ�龭γ�� ��ʹ��lastStr
			regions(i).lng(j) = left$(firstStr,instr(1,firstStr,";") - 1)				'һ����γ������";"�ָ�,";"��ߵı���Ϊlng
			regions(i).lat(j) = right$(firstStr,len(firstStr) - instr(1,firstStr,";")) 	'";"�ұߵı���Ϊlat   Val() ����Ϊ�ַ�ת��Ϊ����
		else
			regions(i).lng(j) = left$(lastStr,instr(1,lastStr,";") - 1)		'һ����γ������";"�ָ�,";"ǰ��ı���Ϊlng
			regions(i).lat(j) = right$(lastStr,len(lastStr) - instr(1,lastStr,";"))		 '";"����ı���Ϊlat Val()Ϊstrת��Ϊnumber
		End If

		'print "lon:____" & regions(i).lng(j)
		'print "lat:____" & regions(i).lat(j)
	Loop While firstStr <> ""


	Redim regions(i).lng(j)		'ȷ������ĳ���
	Redim regions(i).lat(j)	
	regions(i).nodeCount = j		'ȷ�϶���νڵ���
	'print regions(i).nodeCount

next






Create Table "RegionTab" (name Char(254),id Char(254),nodeCount Integer,list Char(254)) file ".\RegionTab.tab" TYPE NATIVE Charset "WindowsSimpChinese"
'����һ��  "RegionTab" ���	������".\RegionTab.tab"		
Create Map For RegionTab CoordSys Earth Projection 1, 0		'��������RegionTab��񴴽�ͼ�� 


'�˶�Ϊ����һ��û�нڵ��Region Ȼ���������regions(i).lng(j) �� regions(i).lat(j)��Region ����ӽڵ�������һ�����������
for i=1 to max_rows Step 1		 

	fetch rec i from RegionTab									'���α��ƶ���RegionTab��ĵ�i��
	Create Region Into Variable regions(i).regObject 0				' ����һ���սڵ��Region����
	
	For j = 1 to regions(i).nodeCount Step 1
		Alter Object regions(i).regObject Node Add (Val(regions(i).lng(j)),Val(regions(i).lat(j)))	'���սڵ��Region��������ӽڵ� 
		regions(i).list = regions(i).list & regions(i).lng(j) & ";" & regions(i).lat(j) & "|"			'�Ѿ�γ������ƴ��Ϊ��γ���б�
	Next
	
	Insert Into RegionTab(name,id,nodeCount,list,Object) 			'��regions(i)���鸴�Ƶ�RegionTab ��
		Values(regions(i).name, regions(i).id, regions(i).nodeCount,regions(i).list,regions(i).regObject)
Next


Commit Table RegionTab Interactive



done: 
  exit sub 
error_trap: 
   print(error$())
   resume done 
end sub








Sub button_exchange
	onerror goto error_trap
	dim re as String
	dim currRegion as object
	dim i ,j,k,selectRowNum as Integer
	Dim tmpRegion() as Region
	dim nodeCount as Integer
	dim lastLon,lastLat,last2ndLon,last2ndLat as string		'���������ľ�γ��,last ��last2nd
    
	Select * from Selection into tmp
	selectRowNum = TableInfo(tmp, TAB_INFO_NROWS)       '��ȡ��ѡ���������
	Redim tmpRegion(selectRowNum)

	for k=1 to selectRowNum  Step 1									   '�ӱ�ĵ�һ�п�ʼ������
		fetch rec k from tmp
		currRegion = 	Selection.obj

        nodeCount = ObjectInfo(currRegion, OBJ_INFO_NPNTS) - 1 		'��ȡ����Ľڵ��� region����ĵ�һ���ڵ�������
        Redim tmpRegion(k).lng(nodeCount)
        Redim tmpRegion(k).lat(nodeCount)

        for i=1 to nodeCount Step 1	 
            tmpRegion(k).lng(i) = Str$(ObjectNodeX(currRegion, 1, i) )				' ��ȡ��i���ڵ�ľ���
            tmpRegion(k).lat(i) = Str$(ObjectNodeY(currRegion, 1, i) )				' ��ȡ��i�����ڵ��γ��		
        next

        lastLon = tmpRegion(k).lng(nodeCount)
        lastLat = tmpRegion(k).lat(nodeCount)
        tmpRegion(k).lng(nodeCount) = tmpRegion(k).lng(nodeCount - 1) 
        tmpRegion(k).lat(nodeCount) = tmpRegion(k).lat(nodeCount - 1)
        tmpRegion(k).lng(nodeCount - 1) = lastLon
        tmpRegion(k).lat(nodeCount - 1) = lastLat







        Create Region Into Variable tmpRegion(k).regObject 0				' ����һ���սڵ��Region����
        
        For j = 1 to nodeCount Step 1
            Alter Object tmpRegion(k).regObject Node Add (Val(tmpRegion(k).lng(j)),Val(tmpRegion(k).lat(j)))		'���սڵ��Region��������ӽڵ� 
            tmpRegion(k).list = tmpRegion(k).list & tmpRegion(k).lng(j) & ";" & tmpRegion(k).lat(j) & "|"			'�Ѿ�γ������ƴ��Ϊ��γ���б�
        Next

        Update tmp Set list = tmpRegion(k).list, obj = tmpRegion(k).regObject    where rowid = k     '���±��obj��Ϊ �´�����Region���� ���±��list��ΪtmpRegion.list


    next
	   
    
	done: 
  		exit sub 
	error_trap: 
   		print(error$())
   	resume done 

end Sub 










Function exchangeNode1(currObj as Object) as integer
	onerror goto error_trap

	dim nodeCount as Integer
	dim lastLon,lastLat,last2ndLon,last2ndLat as float		'���������ľ�γ��,last ��last2nd
	nodeCount = ObjectInfo(currObj, OBJ_INFO_NPNTS) - 1 		'��ȡ����Ľڵ��� region����ĵ�һ���ڵ�������
	lastLon = ObjectNodeX(currObj, 1, nodeCount) 				' ��ȡ���һ���ڵ�ľ���
	lastLat = ObjectNodeY(currObj, 1, nodeCount) 				' ��ȡ���һ���ڵ��γ��
	last2ndLon = ObjectNodeX(currObj, 1, nodeCount - 1) 				' ��ȡ�����ڶ����ڵ�ľ���
	last2ndLat = ObjectNodeY(currObj, 1, nodeCount - 1) 				' ��ȡ�����ڶ����ڵ��γ��

	Alter Object currObj 
		Node Set Position 1, nodeCount (last2ndLon,last2ndLat)			'���õ�����һ�����λ��Ϊ �����ڶ�����ľ�γ��
	Alter Object currObj 
		Node Set Position 1, nodeCount-1 (lastLon,lastLat)			'���õ�����һ�����λ��Ϊ �����ڶ�����ľ�γ��



	

	done: 
  		exit sub 
	error_trap: 
   		print(error$())
   	resume done 

end Function 


