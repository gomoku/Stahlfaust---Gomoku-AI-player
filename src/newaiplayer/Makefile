SOURCES=Communicator.cs Evaluator.cs \
	Player.cs GoBang.cs Coordinate.cs NewAiPlayer.cs \
	StatValEvaluator.cs Threat.cs ThreatSearcher.cs \
	InterestingFieldAgent.cs PrettyPrint.cs \
	../dbsearch/DBSearch.cs \
	../dbsearch/GBSearch.cs ../dbsearch/GBOperators.cs \
	../dbsearch/GBBoard.cs ../dbsearch/GBThreat.cs

FLAGS=-define:STATS

all: bin

bin: GoBang.exe

GoBang.exe: ${SOURCES}
	mcs ${FLAGS} ${SOURCES} -out:GoBang.exe -main:GoBang

TestPrettyPrint.exe:	PrettyPrint.cs
	mcs /out:TestPrettyPrint.exe PrettyPrint.cs

TestFirstSearcher.exe: ${SOURCES} TestFirstSearcher.cs
	mcs ${FLAGS} ${SOURCES} TestFirstSearcher.cs -out:TestFirstSearcher.exe -main:TestFirstSearcher

doc: ${SOURCES}
	doxygen doxyfile

clean:
	rm -rf *exe doc
