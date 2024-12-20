-- This script was generated by the ERD tool in pgAdmin 4.
-- Please log an issue at https://github.com/pgadmin-org/pgadmin4/issues/new/choose if you find any bugs, including reproduction steps.
BEGIN;


CREATE TABLE IF NOT EXISTS public."Clients"
(
    "FIO" text COLLATE pg_catalog."default" NOT NULL,
    "Address" text COLLATE pg_catalog."default" NOT NULL,
    "PhoneNumber" integer NOT NULL,
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( CYCLE INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "DelieveryTypeID" integer NOT NULL,
    CONSTRAINT "Clients_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public."DeliveryMethod"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( CYCLE INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "DelieveryType" text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "DeliveryMethod_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public."Journals"
(
    "Name" text COLLATE pg_catalog."default" NOT NULL,
    "Logo" bytea,
    "LastArrival" date NOT NULL,
    "ReleasePeriod" text COLLATE pg_catalog."default" NOT NULL,
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( CYCLE INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "PublisherID" integer,
    CONSTRAINT "Journals_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public."Publishers"
(
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( CYCLE INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "Name" text COLLATE pg_catalog."default" NOT NULL,
    "Address" text COLLATE pg_catalog."default" NOT NULL,
    "Contact" text COLLATE pg_catalog."default" NOT NULL,
    "Email" text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "Publishers_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public."Subs"
(
    "clientID" integer NOT NULL,
    "journalID" integer NOT NULL,
    "Payment" boolean NOT NULL,
    "CardNumber" integer NOT NULL,
    "SubTime" text COLLATE pg_catalog."default" NOT NULL
);

ALTER TABLE IF EXISTS public."Clients"
    ADD CONSTRAINT "DelieveryType" FOREIGN KEY ("DelieveryTypeID")
    REFERENCES public."DeliveryMethod" (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public."Journals"
    ADD CONSTRAINT "PublisherID" FOREIGN KEY ("PublisherID")
    REFERENCES public."Publishers" (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public."Subs"
    ADD CONSTRAINT "clientID" FOREIGN KEY ("clientID")
    REFERENCES public."Clients" (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public."Subs"
    ADD CONSTRAINT "journalID" FOREIGN KEY ("journalID")
    REFERENCES public."Journals" (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

END;