
CSC=mcs

SOURCES=../dbsearch/DBSearch.cs \
	../dbsearch/GBSearch.cs ../dbsearch/GBOperators.cs \
	../dbsearch/GBBoard.cs ../dbsearch/GBThreat.cs \
	../newaiplayer/PrettyPrint.cs

LIBS=-pkg:gtk-sharp -pkg:glade-sharp #-pkg:rsvg-sharp


all:	fivegui.exe

fivegui.exe:	FiveGUI.cs fivegui.glade $(SOURCES)
	$(CSC) $(CSCFLAGS) -out:fivegui.exe -main:FiveGUI FiveGUI.cs \
		-resource:fivegui.glade \
		$(SOURCES) $(LIBS)

clean:
	rm -f *.exe

