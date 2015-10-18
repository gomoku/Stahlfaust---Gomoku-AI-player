
MCS=mcs

MONO-PROJECT_EXE=mono-project.exe
MONO-PROJECT_SRC=AssemblyInfo.cs \
	MyPicBox.cs \
	WinGui.cs \
	History.cs \
	../dbsearch/DBSearch.cs \
	../dbsearch/GBBoard.cs \
	../dbsearch/GBOperators.cs \
	../dbsearch/GBSearch.cs \
	../dbsearch/GBThreat.cs \
	../newaiplayer/Communicator.cs \
	../newaiplayer/Coordinate.cs \
	../newaiplayer/Evaluator.cs \
	../newaiplayer/InterestingFieldAgent.cs \
	../newaiplayer/NewAiPlayer.cs \
	../newaiplayer/Player.cs \
	../newaiplayer/PrettyPrint.cs \
	../newaiplayer/Threat.cs \
	../newaiplayer/ThreatSearcher.cs \
	../newaiplayer/StatValEvaluator.cs

#MONO-PROJECT_RES=-resource:WinGui.resx,win_project.WinGui.resx

#FLAGS=-define:STATS,PRETTY,MONO

$(MONO-PROJECT_EXE): $(MONO-PROJECT_SRC) 
	$(MCS) ${FLAGS} $(MCSFLAGS) $(LIBS) -main:win_project.Form1 \
		-r:System.dll -r:System.Data.dll -r:System.Drawing.dll \
		-r:System.Windows.Forms.dll -r:System.Xml.dll \
		-target:winexe -out:$(MONO-PROJECT_EXE) \
		$(MONO-PROJECT_RES) $(MONO-PROJECT_SRC)


# common targets

all:	$(MONO-PROJECT_EXE)

# project names as targets

mono-project: $(MONO-PROJECT_EXE)

clean:
	rm -f mono-project.exe
