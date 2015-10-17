
CSC=mcs

SOURCES=DBSearch.cs \
	GBSearch.cs GBOperators.cs GBBoard.cs GBThreat.cs \
	../newaiplayer/PrettyPrint.cs

LIBS=


all:	GBSearchTest.exe GBThreatTest.exe

DLPSearchTest.exe:	DLPSearch.cs $(SOURCES)
	$(CSC) $(CSCFLAGS) -out:DLPSearchTest.exe -main:DLPSearchTest DLPSearch.cs \
		DBSearch.cs $(LIBS)

GBSearchTest.exe:	GBSearch.cs $(SOURCES)
	$(CSC) $(CSCFLAGS) -out:GBSearchTest.exe -main:GBSearchModule \
		$(SOURCES) $(LIBS)

GBThreatTest.exe:	$(SOURCES)
	$(CSC) $(CSCFLAGS) -out:GBThreatTest.exe -main:GBThreatSearch \
		$(SOURCES) $(LIBS)

clean:
	rm -f *.exe

