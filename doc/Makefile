

all:	solution.tex
	latex solution.tex
	latex solution.tex
	dvips -o solution.ps solution.dvi
	gzip -f -9 solution.ps
	pdflatex solution.tex
	pdflatex solution.tex

clean:
	rm -f *.log *.aux *.dvi

